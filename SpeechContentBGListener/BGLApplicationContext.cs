using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovablePython;
using System.Windows.Forms;
using SpeechContent;

namespace SpeechContentBGListener
{
    internal class BGLApplicationContext : ApplicationContext
    {
        private Hotkey hk;
        private Form form;
        private const int SWITCH_USER_COMMAND = 193;

        internal BGLApplicationContext()
        {
            // только создаем форму, она все равно нужна
            // чтобы слушать хоткеи
            form = new Form();

            // создаем и регистрируем глобайльный хоткей
            hk = new Hotkey(Keys.J, false, true, true, false); /////////// <- hotkey

            hk.Pressed += delegate { ActionProcess(); };
            if (hk.GetCanRegister(form))
                hk.Register(form);

            // Вешаем событие на выход
            Application.ApplicationExit += Application_ApplicationExit;
        }
        

        private void ActionProcess()
        {
            string res = "";
            bool isError = false;
            try
            {
                res = "SPEECH CONTENT TO ANDROID\r\n>> " + SpeechContentOps.AddContentFromClipboard();
            }
            catch (Exception e)
            {
                res = "Speech Content Error Add: " + e.Message;
            }
            frmMessage _frmMessage = new frmMessage(res, isError);
            _frmMessage.Show();
        }
        

        void Application_ApplicationExit(object sender, EventArgs e)
        {
            // при выходе разрегистрируем хоткей 
            if (hk.Registered)
                hk.Unregister();
        }
    }
}

