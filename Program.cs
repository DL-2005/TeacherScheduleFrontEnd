using TeacherScheduleFrontend.Forms;
using TeacherScheduleFrontend.Services;

namespace TeacherScheduleFrontend
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            
            // Khởi tạo ApiService
            ApiService.Initialize();
            
            // Hiển thị form đăng nhập
            Application.Run(new FormLogin());
        }
    }
}
