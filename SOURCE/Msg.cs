using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MandelbrotFractal
{
    public class Msg
    {
        public static void Mensagem()
        {
            var msg1 = MessageBox.Show("WARNING: THIS IS MALICIOUS SOFTWARE THAT CAN DESTROY YOUR COMPUTER AND LEAVE IT UNUSABLE.\r\n\r\nIf you click 'YES', you are taking full responsibility for your actions and I, CYBERWARE, have no responsibility for any damages caused. Make sure you fully understand the consequences before proceeding.\r\n\r\nDo you want to continue?",
                "ATTENTION !!",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (msg1 == DialogResult.No)
            {
                CloseApplication();
            }
            else
            {
                var msg2 = MessageBox.Show("By forcing this software to run, you will be sure that it can seriously compromise the functionality of your system. and that CYBERWARE is not responsible for any damages, loss of data or any other negative consequences resulting from the use of this software.\r\n\r\nAre you aware of the risks and want to continue?",
                    "TheNeRiNe.EXE - !! LAST WARNING !!",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (msg2 == DialogResult.No)
                {
                    CloseApplication();
                }
            }
        }

        public static void CloseApplication()
        {
            string exeName = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
            var processes = Process.GetProcessesByName(exeName);
            foreach (var process in processes)
            {
                process.Kill();
            }
        }
    }
}
