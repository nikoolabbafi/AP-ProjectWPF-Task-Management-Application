using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.ViewModel;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public ToDoListViewModel ViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new ToDoListViewModel();
            DataContext = ViewModel;

            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadData();
        }

        public async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var newTask = new Model.Task
            {
                Name = "New Task",
                Checked = false,
                Starred = false,
                ListId = 1,
                DueDate = DateTime.Now.ToString(
                    "M/d/yyyy h:mm:ss tt",
                    CultureInfo.InvariantCulture),
                ListName = "Nothing"
            };

            bool success = await ViewModel.AddTask(newTask);
            if (!success)
                MessageBox.Show("Error adding task");
        }

        public async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.DataContext is not Task2 task)
                return;

            var updatedTask = new Model.Task
            {
                Id = task.Id,
                Name = task.Name,
                Checked = task.Checked,
                Starred = task.Starred,
                ListId = task.ListId,
                ListName = task.ListName,
                DueDate = task.DueDate.HasValue
                    ? task.DueDate.Value.ToString(
                        "M/d/yyyy h:mm:ss tt",
                        CultureInfo.InvariantCulture)
                    : string.Empty
            };

            try
            {
                bool success = await ViewModel.EditTask(updatedTask);

                if (!success)
                {
                    MessageBox.Show("Error editing task");
                    return;
                }

                await ViewModel.LoadData();
                MessageBox.Show("Task updated successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing task: {ex.Message}");
            }
        }

        public async void EditListButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.DataContext is not Task2 task)
                return;

            try
            {
                bool success = await ViewModel.ApplyListName(task);

                if (!success)
                {
                    MessageBox.Show(
                        "Could not update or assign the list. " +
                        "If this is a new list name, make sure POST /api/lists exists in the API.");

                    await ViewModel.LoadData();
                    return;
                }

                await ViewModel.LoadData();
                MessageBox.Show("List updated successfully!");
            }
            catch (Exception ex)
            {
                await ViewModel.LoadData();
                MessageBox.Show($"Error updating list: {ex.Message}");
            }
        }

        // Use this if your XAML is changed to Click="CheckBox_Click".
        private async void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            await SaveCheckBoxAsync(sender);
        }

        // Kept so the original XAML with Checked/Unchecked also still compiles.
        // Prefer CheckBox_Click in XAML to avoid updates during data binding/load.
        private async void CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            await SaveCheckBoxAsync(sender);
        }

        private async Task SaveCheckBoxAsync(object sender)
        {
            if (sender is not CheckBox checkBox || checkBox.DataContext is not Task2 task)
                return;

            try
            {
                bool success = await ViewModel.UpdateTask(task);

                if (!success)
                {
                    await ViewModel.LoadData();
                    MessageBox.Show("Error updating task");
                }
            }
            catch (Exception ex)
            {
                await ViewModel.LoadData();
                MessageBox.Show($"Error updating task: {ex.Message}");
            }
        }

        public async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.DataContext is not Task2 task)
                return;

            try
            {
                bool isDeleted = await ViewModel.DeleteTask(task.Id);

                if (isDeleted)
                {
                    ViewModel.MyTasks.Remove(task);
                }
                else
                {
                    MessageBox.Show("Error deleting task");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting task: {ex.Message}");
            }
        }
    }
}
