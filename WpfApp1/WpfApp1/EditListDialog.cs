using System.Windows;
using WpfApp1.Model;

public class EditListDialog : Window
{

    

    public List EditedList { get; private set; }

    public EditListDialog(List listToEdit)
    {
        EditedList = new List
        {
            Id = listToEdit.Id,
            Name = listToEdit.Name
        };

        DataContext = EditedList;

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
