using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TwitchLib;
using TwitchLib.Models.API.Undocumented.Chatters;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private ConcurrentBag<ChatterFormatted> _users;

        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // our twitch client id.
            TwitchAPI.Settings.ClientId = ""; // add your Twitch Client Id.
            _users = new ConcurrentBag<ChatterFormatted>();
        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            // Get Chatters from Api
            var chatters = await TwitchAPI.Undocumented.GetChatters("burkeblack");

            //If Chatters request was OK - load into concurrentbag and bind to grid.
            if (chatters is List<ChatterFormatted>)
            {
                var newBag = new ConcurrentBag<ChatterFormatted>();
                // Replaces old bag with new bag - thread safe.
                Interlocked.Exchange(ref _users, newBag);
                //add chatters from the response list object to the newbag.
                foreach (var user in chatters)
                {
                    _users.Add(user);
                }
                BindGrid();
            }
        }

        private void BindGrid()
        {
            //clear columns if you are rebinding
            if (usersGrid.ColumnCount > 0)
            {
                usersGrid.Columns.Clear();
            }

            //stop it making columns
            usersGrid.AutoGenerateColumns = false;

            //create the username column programatically
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            DataGridViewTextBoxColumn colUsername = new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                Name = "Username",
                HeaderText = "Username",
                DataPropertyName = "Username" // Tell the column which property of ChatterFormatted it should use
            };

            usersGrid.Columns.Add(colUsername);

            var bindingList = new BindingList<ChatterFormatted>(_users.ToList()); // <-- BindingList
            usersGrid.DataSource = bindingList; // Bind it to the grid.
}
    }
}
