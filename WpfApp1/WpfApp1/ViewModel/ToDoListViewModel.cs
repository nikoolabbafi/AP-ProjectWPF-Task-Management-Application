using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

using TodoList = WpfApp1.Model.List;
using TodoTask = WpfApp1.Model.Task;

namespace WpfApp1.ViewModel
{
    public class Task2 : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private bool _checked;
        private bool _starred;
        private DateTime? _dueDate;
        private string _listName = string.Empty;
        private int _listId;

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        [JsonPropertyName("checked")]
        public bool Checked
        {
            get => _checked;
            set { _checked = value; OnPropertyChanged(); }
        }

        [JsonPropertyName("starred")]
        public bool Starred
        {
            get => _starred;
            set { _starred = value; OnPropertyChanged(); }
        }

        [JsonPropertyName("listId")]
        public int ListId
        {
            get => _listId;
            set { _listId = value; OnPropertyChanged(); }
        }

        [JsonPropertyName("dueDate")]
        public DateTime? DueDate
        {
            get => _dueDate;
            set { _dueDate = value; OnPropertyChanged(); }
        }

        [JsonPropertyName("listname")]
        public string ListName
        {
            get => _listName;
            set { _listName = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ToDoListViewModel : INotifyPropertyChanged
    {
        private readonly TaskDataProvider dataprovider;
        private readonly ListDataProvider listDataProvider;
        private TodoTask? selectedTask;

        public ObservableCollection<Task2?> MyTasks { get; } = new();
        public ObservableCollection<TodoList?> Lists { get; } = new();

        public TodoTask? SelectedTask
        {
            get => selectedTask;
            set
            {
                selectedTask = value;
                OnPropertyChanged();
            }
        }

        public ToDoListViewModel()
        {
            dataprovider = new TaskDataProvider();
            listDataProvider = new ListDataProvider();
        }

        public async Task LoadData()
        {
            try
            {
                var taskList = await dataprovider.getAllTodos() ?? new List<TodoTask>();
                var listList = await listDataProvider.getAllLists() ?? new List<TodoList>();

                MyTasks.Clear();

                foreach (var task in taskList)
                {
                    var relatedList = listList.FirstOrDefault(x => x.Id == task.ListId);

                    MyTasks.Add(new Task2
                    {
                        Id = task.Id,
                        Name = task.Name ?? string.Empty,
                        Checked = task.Checked,
                        Starred = task.Starred,
                        ListId = task.ListId,
                        ListName = relatedList?.Name ?? "Unknown List",
                        DueDate = ParseDueDate(task.DueDate)
                    });
                }

                Lists.Clear();
                foreach (var list in listList)
                    Lists.Add(list);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
        }

        private static DateTime? ParseDueDate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (DateTime.TryParseExact(
                    value,
                    "M/d/yyyy h:mm:ss tt",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var exact))
            {
                return exact;
            }

            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var invariant))
                return invariant;

            if (DateTime.TryParse(value, out var currentCulture))
                return currentCulture;

            return null;
        }

        private static string FormatDueDate(DateTime? value)
        {
            return value.HasValue
                ? value.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                : string.Empty;
        }

        public async Task<bool> AddTask(TodoTask task)
        {
            bool isAdded = await dataprovider.AddTask(task);
            if (isAdded)
                await LoadData();

            return isAdded;
        }

        public Task<bool> DeleteTask(int taskId)
        {
            return dataprovider.DeleteTask(taskId);
        }

        public Task<bool> EditTask(TodoTask updatedTask)
        {
            return dataprovider.EditTask(updatedTask);
        }

        // Used by checkbox/date/list UI where DataContext is Task2.
        public async Task<bool> UpdateTask(Task2 task)
        {
            if (task == null)
                return false;

            var updatedTask = new TodoTask
            {
                Id = task.Id,
                Name = task.Name,
                Checked = task.Checked,
                Starred = task.Starred,
                ListId = task.ListId,
                ListName = task.ListName,
                DueDate = FormatDueDate(task.DueDate)
            };

            return await UpdateTask(updatedTask);
        }

        // Kept as an overload so old callers that already have Model.Task still compile.
        public async Task<bool> UpdateTask(TodoTask updatedTask)
        {
            bool isSuccess = await dataprovider.UpdateTask(updatedTask);

            if (isSuccess)
            {
                var localTask = MyTasks.FirstOrDefault(t => t?.Id == updatedTask.Id);
                if (localTask != null)
                {
                    localTask.Name = updatedTask.Name ?? string.Empty;
                    localTask.Checked = updatedTask.Checked;
                    localTask.Starred = updatedTask.Starred;
                    localTask.ListId = updatedTask.ListId;
                    localTask.ListName = updatedTask.ListName ?? localTask.ListName;
                    localTask.DueDate = ParseDueDate(updatedTask.DueDate);
                }
            }

            return isSuccess;
        }

        public Task<bool> EditList(TodoList updatedList)
        {
            Debug.WriteLine(JsonSerializer.Serialize(updatedList));
            return listDataProvider.EditList(updatedList);
        }

        // This is the missing method from the compiler error.
        // Unknown List -> assign to an existing list or create a new list.
        // Existing list -> rename that list, so every task with the same ListId gets the new name.
        public async Task<bool> ApplyListName(Task2 task)
        {
            if (task == null)
                return false;

            string requestedListName = (task.ListName ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(requestedListName) ||
                requestedListName.Equals("Unknown List", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // If this task already points to a real list, Edit List means rename that list.
            var currentList = Lists.FirstOrDefault(l => l != null && l.Id == task.ListId);

            if (currentList != null)
            {
                if (string.Equals(currentList.Name, requestedListName, StringComparison.Ordinal))
                    return true;

                string oldName = currentList.Name;
                currentList.Name = requestedListName;

                bool renamed = await listDataProvider.EditList(currentList);
                if (!renamed)
                {
                    currentList.Name = oldName;
                    return false;
                }

                // Keep all rows that share this ListId visually synchronized immediately.
                foreach (var item in MyTasks.Where(t => t?.ListId == currentList.Id))
                {
                    if (item != null)
                        item.ListName = requestedListName;
                }

                return true;
            }

            // The task has no valid list. Try to attach it to an existing list with this name.
            var targetList = Lists.FirstOrDefault(l =>
                l != null &&
                string.Equals(l.Name?.Trim(), requestedListName, StringComparison.OrdinalIgnoreCase));

            // If it does not exist, create it through POST /api/lists.
            if (targetList == null)
            {
                targetList = await listDataProvider.AddList(requestedListName);
                if (targetList == null)
                    return false;

                if (!Lists.Any(l => l?.Id == targetList.Id))
                    Lists.Add(targetList);
            }

            int oldListId = task.ListId;
            string oldListName = task.ListName;

            task.ListId = targetList.Id;
            task.ListName = targetList.Name;

            bool taskUpdated = await UpdateTask(task);
            if (!taskUpdated)
            {
                task.ListId = oldListId;
                task.ListName = oldListName;
            }

            return taskUpdated;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ListDataProvider
    {
        private readonly HttpClient httpClient = new();

        public Task<List<TodoList>?> getAllLists()
        {
            return httpClient.GetFromJsonAsync<List<TodoList>?>("https://localhost:7151/api/lists");
        }

        public async Task<bool> EditList(TodoList updatedList)
        {
            Debug.WriteLine(JsonSerializer.Serialize(updatedList));
            var response = await httpClient.PutAsJsonAsync(
                $"https://localhost:7151/api/lists/{updatedList.Id}",
                updatedList);

            return response.IsSuccessStatusCode;
        }

        public async Task<TodoList?> AddList(string listName)
        {
            var newList = new TodoList { Name = listName };

            var response = await httpClient.PostAsJsonAsync(
                "https://localhost:7151/api/lists",
                newList);

            if (!response.IsSuccessStatusCode)
                return null;

            // Best case: API returns the created object (usually 200/201).
            try
            {
                if (response.Content.Headers.ContentLength.GetValueOrDefault() > 0)
                {
                    var created = await response.Content.ReadFromJsonAsync<TodoList>();
                    if (created != null && created.Id > 0)
                        return created;
                }
            }
            catch
            {
                // Some APIs return no JSON body after create; fall back to reloading below.
            }

            // Fallback: reload and locate the newly-created list by name.
            var allLists = await getAllLists();
            return allLists?
                .Where(l => string.Equals(l.Name, listName, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(l => l.Id)
                .FirstOrDefault();
        }
    }

    public class TaskDataProvider
    {
        private readonly HttpClient httpClient = new();

        public Task<List<TodoTask>?> getAllTodos()
        {
            return httpClient.GetFromJsonAsync<List<TodoTask>?>("https://localhost:7151/api/tasks");
        }

        public async Task<bool> AddTask(TodoTask newTask)
        {
            Debug.WriteLine(JsonSerializer.Serialize(newTask));
            var response = await httpClient.PostAsJsonAsync(
                "https://localhost:7151/api/tasks",
                newTask);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> EditTask(TodoTask updatedTask)
        {
            Debug.WriteLine(JsonSerializer.Serialize(updatedTask));
            var response = await httpClient.PutAsJsonAsync(
                $"https://localhost:7151/api/tasks/{updatedTask.Id}",
                updatedTask);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateTask(TodoTask updatedTask)
        {
            var response = await httpClient.PutAsJsonAsync(
                $"https://localhost:7151/api/tasks/{updatedTask.Id}",
                updatedTask);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTask(int taskId)
        {
            try
            {
                var response = await httpClient.DeleteAsync(
                    $"https://localhost:7151/api/tasks/{taskId}");

                if (response.IsSuccessStatusCode)
                    return true;

                var responseContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Delete failed. Status: {response.StatusCode}; Response: {responseContent}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Delete error for task {taskId}: {ex.Message}");
                return false;
            }
        }
    }
}
