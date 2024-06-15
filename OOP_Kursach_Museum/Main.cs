using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace OOP_Kursach_Guest
{
    /// <summary>
    /// Основная форма приложения для управления музейными экспонатами.
    /// </summary>
    public partial class Main : Form
    {
        private List<Guest> users = new List<Guest>();
        private ContextMenuStrip contextMenuStrip;

        /// <summary>
        /// Конструктор формы.
        /// </summary>
        public Main()
        {
            InitializeComponent();
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            dataGridView1.CurrentCellDirtyStateChanged += dataGridView1_CurrentCellDirtyStateChanged;
            buttonDeleteDatabase.Click += buttonDeleteDatabase_Click;
            button2.Click += button2_Click; // Добавляем обработчик для button2
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            textBox1.TextChanged += TextBox1_TextChanged;
            ComboBoxFilterBySurname.SelectedIndexChanged += FilterComboBoxes_Changed;
            ComboBoxFilterByRoom.SelectedIndexChanged += FilterComboBoxes_Changed;
            checkBoxFilterOnExhibit.CheckedChanged += FilterComboBoxes_Changed;

            contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.Add("Удалить").Click += Delete_Click;
            dataGridView1.ContextMenuStrip = contextMenuStrip;
            dataGridView1.MouseDown += DataGridView1_MouseDown;
        }

        /// <summary>
        /// Обработчик события загрузки формы.
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            DataTable table = new DataTable();
            table.Columns.Add("Фамилия", typeof(string));
            table.Columns.Add("Имя", typeof(string));
            table.Columns.Add("Телефон", typeof(long));
            table.Columns.Add("№ комнаты", typeof(int));
            table.Columns.Add("Заселен", typeof(bool));

            users = FileManager.ReadFromFile();
            foreach (var user in users)
            {
                table.Rows.Add(user.Surname, user.Name, user.PhoneNumber, user.Room, user.OnLiving);
            }

            dataGridView1.DataSource = table;
            UpdateFilterComboBoxes();
            UpdateExhibitCount();
        }

        /// <summary>
        /// Обновляет значения в комбобоксах фильтров.
        /// </summary>
        private void UpdateFilterComboBoxes()
        {
            ComboBoxFilterBySurname.Items.Clear();
            ComboBoxFilterBySurname.Items.Add("");
            foreach (var user in users)
            {
                if (!ComboBoxFilterBySurname.Items.Contains(user.Surname))
                {
                    ComboBoxFilterBySurname.Items.Add(user.Surname);
                }
            }

            ComboBoxFilterByRoom.Items.Clear();
            ComboBoxFilterByRoom.Items.Add("");
            foreach (var user in users)
            {
                if (!ComboBoxFilterByRoom.Items.Contains(user.Room.ToString()))
                {
                    ComboBoxFilterByRoom.Items.Add(user.Room.ToString());
                }
            }
        }

        /// <summary>
        /// Обработчик клика по ячейке DataGridView.
        /// </summary>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBoxName.Text = row.Cells["Имя"].Value.ToString();
                textBoxSurname.Text = row.Cells["Фамилия"].Value.ToString();
                textBoxPhoneNumber.Text = row.Cells["Телефон"].Value.ToString();
                listBoxRoom.Text = row.Cells["№ комнаты"].Value.ToString();
                checkBoxOnLiving.Checked = (bool)row.Cells["Заселен"].Value;
            }
        }

        /// <summary>
        /// Обработчик клика по кнопке добавления экспоната.
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if ((textBoxPhoneNumber.Text.Length==11 || !string.IsNullOrEmpty(textBoxName.Text) || !string.IsNullOrEmpty(textBoxPhoneNumber.Text) || !string.IsNullOrEmpty(textBoxSurname.Text)) && int.TryParse(listBoxRoom.Text, out int room) && textBoxPhoneNumber.Text.All(Char.IsDigit))
            {
            string name = textBoxName.Text;
            string surname = textBoxSurname.Text;
            room = Convert.ToInt32(listBoxRoom.Text);
            long phonenumber = Convert.ToInt64(textBoxPhoneNumber.Text);
            bool onLiving = checkBoxOnLiving.Checked;
                if (roomnumberchecker(room))
                {
                    var Guest = new Guest(name, surname, phonenumber, room, onLiving);
                    users.Add(Guest);
                    DataTable table = (DataTable)dataGridView1.DataSource;
                    table.Rows.Add(surname, name, phonenumber, room, onLiving);
                    FileManager.AppendToFile(Guest);
                    UpdateExhibitCount();
                }
                else
                {
                    MessageBox.Show("Не может быть заселено более 3х человек в номер!");
                }
            }
            else
            {
                MessageBox.Show("Не должно быть пустых строк и телефон должен состоять из 11 цифр!");
            }
            UpdateFilterComboBoxes();
        }

        /// <summary>
        /// Записывает данные пользователей в файл.
        /// </summary>
        private void WriteDataToFile()
        {
            FileManager.WriteToFile(users);
            UpdateFilterComboBoxes();
        }

        /// <summary>
        /// Обработчик изменения фильтров.
        /// </summary>
        private void FilterComboBoxes_Changed(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Обработчик изменения текста в текстовом поле поиска.
        /// </summary>
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Применяет фильтры к списку пользователей.
        /// </summary>
        private void ApplyFilters()
        {
            string filterBySurname = ComboBoxFilterBySurname.Text.Trim();
            string filterByRoomText = ComboBoxFilterByRoom.Text.Trim();
            bool filterByRoomEnabled = int.TryParse(filterByRoomText, out int filterByRoom);
            bool filterByOnLiving = checkBoxFilterOnExhibit.Checked;

            var filteredUsers = users;
            if (!string.IsNullOrWhiteSpace(filterBySurname))
            {
                filteredUsers = filteredUsers.Where(u => u.Surname.Contains(filterBySurname)).ToList();
            }

            if (filterByRoomEnabled)
            {
                filteredUsers = filteredUsers.Where(u => u.Room == filterByRoom).ToList();
            }

            if (filterByOnLiving)
            {
                filteredUsers = filteredUsers.Where(u => u.OnLiving).ToList();
            }

            string searchText = textBox1.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                filteredUsers = filteredUsers.Where(u => u.Surname.Contains(searchText) || u.Room.ToString().Contains(searchText) || u.Name.Contains(searchText) || u.PhoneNumber.ToString().Contains(searchText)).ToList();
            }

            UpdateData(filteredUsers);
            UpdateExhibitCount();
        }

        /// <summary>
        /// Обновляет данные в DataGridView.
        /// </summary>
        private void UpdateData(List<Guest> filteredUsers)
        {
            DataTable table = (DataTable)dataGridView1.DataSource;
            table.Rows.Clear();
            foreach (var user in filteredUsers)
            {
                table.Rows.Add(user.Surname, user.Name, user.PhoneNumber, user.Room, user.OnLiving);
            }
        }

        /// <summary>
        /// Обновляет количество экспонатов.
        /// </summary>
        private void UpdateExhibitCount()
        {
            labelCount.Text = $"Общее количество гостей: {dataGridView1.Rows.Count}";
            int livingCount = users.Count(u => u.OnLiving);
            labelExhibitCount.Text = $"Проживает гостей: {livingCount}";
        }

        /// <summary>
        /// Обработчик клика по кнопке удаления базы данных.
        /// </summary>
        private void buttonDeleteDatabase_Click(object sender, EventArgs e)
        {
            users.Clear();
            DataTable table = (DataTable)dataGridView1.DataSource;
            table.Rows.Clear();
            FileManager.DeleteFile();
            UpdateExhibitCount();
            UpdateFilterComboBoxes();
        }

        /// <summary>
        /// Обработчик изменения значения ячейки DataGridView.
        /// </summary>
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string name = dataGridView1.Rows[e.RowIndex].Cells["Имя"].Value.ToString();
                string surname = dataGridView1.Rows[e.RowIndex].Cells["Фамилия"].Value.ToString();
                long phonenumber = (int)dataGridView1.Rows[e.RowIndex].Cells["Телефон"].Value;
                int room = (int)dataGridView1.Rows[e.RowIndex].Cells["№ комнаты"].Value;
                bool onLiving = (bool)dataGridView1.Rows[e.RowIndex].Cells["Заселен"].Value;

                users[e.RowIndex] = new Guest(name, surname, phonenumber, room, onLiving);
                WriteDataToFile();
                UpdateExhibitCount();
            }
        }

        /// <summary>
        /// Обработчик изменения состояния редактируемой ячейки DataGridView.
        /// </summary>
        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        /// <summary>
        /// Обработчик нажатия правой кнопкой мыши на DataGridView.
        /// </summary>
        private void DataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hit = dataGridView1.HitTest(e.X, e.Y);
                dataGridView1.ClearSelection();
                if (hit.RowIndex >= 0)
                {
                    dataGridView1.Rows[hit.RowIndex].Selected = true;
                    contextMenuStrip.Show(dataGridView1, e.Location);
                }
            }
        }

        /// <summary>
        /// Обработчик клика по пункту контекстного меню "Удалить".
        /// </summary>
        private void Delete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int index = dataGridView1.SelectedRows[0].Index;
                users.RemoveAt(index);
                dataGridView1.Rows.RemoveAt(index);
                WriteDataToFile();
                UpdateExhibitCount();
                UpdateFilterComboBoxes();
            }
        }

        /// <summary>
        /// Обработчик клика по кнопке редактирования экспоната.
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
            {
                if (textBoxPhoneNumber.Text.All(Char.IsDigit) && int.TryParse(listBoxRoom.Text, out int room))
                {
                    long phonenumber = Convert.ToInt64(textBoxPhoneNumber.Text);
                    if (roomnumberchecker(room))
                    {
                        int selectedIndex = dataGridView1.CurrentRow.Index;
                        // Обновление DataTable
                        DataTable table = (DataTable)dataGridView1.DataSource;
                        table.Rows[selectedIndex]["Имя"] = textBoxName.Text;
                        table.Rows[selectedIndex]["Фамилия"] = textBoxSurname.Text;
                        table.Rows[selectedIndex]["Телефон"] = phonenumber;
                        table.Rows[selectedIndex]["№ комнаты"] = listBoxRoom.SelectedItem;
                        table.Rows[selectedIndex]["Заселен"] = checkBoxOnLiving.Checked;

                        // Обновление списка users
                        users[selectedIndex] = new Guest(textBoxName.Text, textBoxSurname.Text, phonenumber, Convert.ToInt32(listBoxRoom.Text), checkBoxOnLiving.Checked);

                        // Запись обновленных данных обратно в файл
                        WriteDataToFile();

                        // Обновление количества экспонатов
                        UpdateExhibitCount();
                    }
                    else
                    {
                        MessageBox.Show("В номере не может быть более 3-х человек!");
                        listBoxRoom.Text = string.Empty; // Возвращаем предыдущее значение
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите строку для редактирования.");
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonDeleteDatabase_Click_1(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private bool roomnumberchecker(int roomnumber)
        {
            int count = 0;
            foreach (Guest guest in users)
            {
                if (guest.Room == roomnumber && guest.OnLiving) count++;
                if (count == 3 && checkBoxOnLiving.Checked) return false;
            }
            return true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void checkBoxOnLiving_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
