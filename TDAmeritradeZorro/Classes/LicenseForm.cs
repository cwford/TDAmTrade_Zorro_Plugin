using System.Windows.Forms;
using TDAmeritradeZorro.Utilities;
using TDAmeritradeZorro.Classes;

namespace TDAmeritradeZorro.Classes
{
    public partial class LicenseForm : Form
    {
        public LicenseForm()
        {
            InitializeComponent();

            this.icon = Helper.GetWindowsFormIcon(FormType.License);
            this.Icon = icon;
        }
    }
}
