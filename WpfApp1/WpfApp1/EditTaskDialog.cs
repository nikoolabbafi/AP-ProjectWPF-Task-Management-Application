using System;
using System.Windows;
using WpfApp1.Model;

namespace WpfApp1
{
    public class EditTaskDialog : Window
    {
        private Model.Task editedTask;

        public EditTaskDialog(Model.Task selectedTask)
        {
           
            editedTask = new Model.Task
            {
                Id = selectedTask.Id,
                Name = selectedTask.Name,
                Checked = selectedTask.Checked,
                Starred = selectedTask.Starred,
                ListId = selectedTask.ListId,
                DueDate = selectedTask.DueDate
            };

            DataContext = editedTask;
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
