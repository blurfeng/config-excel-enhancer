using System.ComponentModel;

namespace ConfigExcelEnhancer.UI
{
    public partial class ExcelPickerControl : UserControl
    {
        public event EventHandler? ValueChanged;

        private bool _suppressEvents;

        public ExcelPickerControl()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string LabelText
        {
            get => lblTitle.Text;
            set
            {
                lblTitle.Text = value;
                pnlRadioGroup.Left = lblTitle.PreferredWidth + 8;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Mode
        {
            get => rdoList.Checked ? 1 : 0;
            set
            {
                _suppressEvents = true;
                if (value == 1)
                    rdoList.Checked = true;
                else
                    rdoDirectory.Checked = true;
                _suppressEvents = false;
                UpdateModeVisibility();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ExcelDirectory
        {
            get => txtDir.Text;
            set
            {
                _suppressEvents = true;
                txtDir.Text = value;
                _suppressEvents = false;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<string> Files
        {
            get => lstFiles.Items.Cast<string>().ToList();
            set
            {
                _suppressEvents = true;
                lstFiles.Items.Clear();
                foreach (var f in value)
                    lstFiles.Items.Add(f);
                _suppressEvents = false;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SearchOption DirectorySearchOption { get; set; } = SearchOption.TopDirectoryOnly;

        public List<string> BuildFileList(string? excludeFilePath = null)
        {
            if (rdoDirectory.Checked)
            {
                string dir = txtDir.Text.Trim();
                if (!Directory.Exists(dir))
                    return new List<string>();

                string? excludeFull = string.IsNullOrEmpty(excludeFilePath)
                    ? null
                    : Path.GetFullPath(excludeFilePath);

                return Directory.EnumerateFiles(dir, "*.xlsx", DirectorySearchOption)
                    .Where(f => !Path.GetFileName(f).StartsWith("~$"))
                    .Where(f => excludeFull == null ||
                        !string.Equals(Path.GetFullPath(f), excludeFull, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                return lstFiles.Items.Cast<string>()
                    .Where(File.Exists)
                    .ToList();
            }
        }

        private void UpdateModeVisibility()
        {
            pnlDirMode.Visible = rdoDirectory.Checked;
            pnlListMode.Visible = rdoList.Checked;
        }

        private void rdoDirectory_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoDirectory.Checked)
            {
                UpdateModeVisibility();
                if (!_suppressEvents) ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void rdoList_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoList.Checked)
            {
                UpdateModeVisibility();
                if (!_suppressEvents) ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void txtDir_TextChanged(object sender, EventArgs e)
        {
            if (!_suppressEvents) ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (DialogHelper.BrowseFolder("选择 Excel 目录", txtDir.Text) is { } path)
                txtDir.Text = path;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var files = DialogHelper.BrowseFiles(
                "选择 Excel 文件",
                "Excel 文件 (*.xlsx)|*.xlsx",
                multiselect: true);
            if (files.Length > 0)
            {
                foreach (var f in files)
                    if (!lstFiles.Items.Contains(f))
                        lstFiles.Items.Add(f);
                if (!_suppressEvents) ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach (var item in lstFiles.SelectedItems.Cast<string>().ToList())
                lstFiles.Items.Remove(item);
            if (!_suppressEvents) ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstFiles.Items.Clear();
            if (!_suppressEvents) ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
