using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.Json;


namespace WpfApp1.Model
{
    
    public class Task : INotifyPropertyChanged
    {
        private string name;
        private bool isChecked;
        private bool isStarred;
        private string dueDate;

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName( "name")]
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        [JsonPropertyName( "checked")]
        public bool Checked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                OnPropertyChanged(nameof(Checked));
            }
        }

        [JsonPropertyName("starred")]
        public bool Starred
        {
            get => isStarred;
            set
            {
                isStarred = value;
                OnPropertyChanged(nameof(Starred));
            }
        }


        [JsonPropertyName("listId")]
        public int ListId { get; set; }


        [JsonPropertyName("dueDate")]
        public string DueDate
        {
            get => dueDate;
            set
            {
                dueDate = value;
                OnPropertyChanged(nameof(DueDate));
            }
        }
       

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private string listName;

        [JsonPropertyName("listname")] 
        public string ListName
        {
            get => listName;
            set
            {
                listName = value;
                OnPropertyChanged(nameof(ListName));
            }
        }
    }

}
