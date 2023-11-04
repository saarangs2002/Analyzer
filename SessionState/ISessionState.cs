using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionState
{
    public interface ISessionState
    {
        ObservableCollection<Student> GetAllStudents();
        void AddNewStudent(int id, string name, string ip, int port);
        void RemoveStudent(int id);
    }
}
