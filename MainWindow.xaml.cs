using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Reflection.PortableExecutable;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace ADO.NET_TASK_3;
public partial class MainWindow : Window,INotifyPropertyChanged
{
    public ObservableCollection<string> Authors { get; set; } = new();
    public ObservableCollection<Book> Books { get => books; set{ books = value; OnPropertyChanged(); } }
    private readonly IConfiguration _configuration;
    private SqlConnection connection;
    private DataSet dataSet;
    private SqlDataAdapter adapter;
    private SqlCommandBuilder cmd;
    private ObservableCollection<Book> books = new();

    public MainWindow(IConfiguration configuration)
    {
        InitializeComponent();
        DataContext = this;
        _configuration = configuration;
        connection = new();
        connection.ConnectionString = _configuration.GetConnectionString("DbConnection")!;
        ReadAUthors();

    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        dataSet = new();
        var combobox = sender as ComboBox;
        if (combobox is null || combobox.SelectedItem is null)
            return;
        Books=new();
        var fullname = combobox?.SelectedItem.ToString();
        var name = fullname.Split(' ');

        string query = $@"SELECT *
                          FROM Books AS B
                          JOIN Authors AS A ON B.Id_Author=A.ID
                          WHERE A.FIRSTNAME+' '+A.LastName LIKE  '%{fullname}%'";
        adapter = new SqlDataAdapter(query, connection);
        cmd = new(adapter);
        adapter.Fill(dataSet, "books");
        for (int i = 0; i < dataSet.Tables["books"]?.Rows.Count; i++)
        {
            DataRow row = dataSet.Tables["books"]?.Rows[i]!;
            Books.Add(new Book { 
                                 Id= Convert.ToInt32(row["Id"]), 
                                 Pages= Convert.ToInt32(row["Pages"]), 
                                 Id_Author= Convert.ToInt32(row["Id_Author"]), 
                                 Id_Category= Convert.ToInt32(row["Id_Category"]), 
                                 Id_Themes= Convert.ToInt32(row["Id_Themes"]), 
                                 Id_Press= Convert.ToInt32(row["Id_Press"]), 
                                 YearPress= Convert.ToInt32(row["YearPress"]), 
                                 Quantity= Convert.ToInt32(row["Quantity"]), 
                                 Name= row["Name"].ToString()!,
                                 Comment= row["Comment"].ToString()!
            });
        }
    }

    void ReadAUthors()
    {
        dataSet = new();
        string query = "SELECT Firstname,Lastname FROM AUTHORS ";
        adapter = new SqlDataAdapter(query, connection);
        cmd = new(adapter);
        adapter.Fill(dataSet, "authors");
        for (int i = 0; i < dataSet.Tables["authors"]?.Rows.Count; i++)
        {
            DataRow row = dataSet.Tables["authors"]?.Rows[i]!;
            Authors.Add($"{row["Firstname"]} {row["Lastname"]}");
        }
    }


}