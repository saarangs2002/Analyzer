using System.Collections.ObjectModel;

namespace SessionState
{
    public class StudentSessionState
    {
        private ObservableCollection<Student> students;

        public StudentSessionState()
        {
            students = new ObservableCollection<Student>();
        }

        public void AddStudent(int id, string name, string ip, int port)
        {
            var check = students.FirstOrDefault(s => s.Id == id);
            if (check == null)
            {
                var student = new Student
                {
                    Id = id,
                    Name = name,
                    IP = ip,
                    Port = port
                };
                students.Add(student);
            }
        }

        public void RemoveStudent(int id)
        {
            var studentToRemove = students.FirstOrDefault(s => s.Id == id);
            if (studentToRemove != null)
            {
                students.Remove(studentToRemove);
            }
        }

        public ObservableCollection<Student> GetAllStudents()
        {
            return students;
        }
    }
}
