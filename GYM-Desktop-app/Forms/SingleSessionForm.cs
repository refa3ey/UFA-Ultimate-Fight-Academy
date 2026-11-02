using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using GYM_Desktop_app.Database;
using GYM_Desktop_app.Models;

namespace GYM_Desktop_app.Forms
{
    // One screen for both dashboard buttons: a new person's free first session,
    // or one paid session for anyone, with the money going to the chosen coach.
    public class SingleSessionForm : Form
    {
        private readonly bool _isFree;
        private bool  _dragging;
        private Point _dragStart;

        private readonly ComboBox     cmbPerson = NewCombo();
        private readonly Guna2TextBox txtName   = NewTextBox();
        private readonly Guna2TextBox txtPhone  = NewTextBox();
        private readonly ComboBox     cmbCoach  = NewCombo();
        private readonly NumericUpDown numAmount = new NumericUpDown { Font = new Font("Segoe UI", 10F), Maximum = 1000000, Size = new Size(380, 30) };
        private readonly ComboBox     cmbMethod = NewCombo();

        public SingleSessionForm(bool isFree)
        {
            _isFree = isFree;
            BuildUi();
            LoadData();
        }

        private void BuildUi()
        {
            string title = _isFree ? "Free First Session" : "Pay for One Session";
            Text            = title;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition   = FormStartPosition.CenterParent;
            BackColor       = Color.White;

            var top = new Guna2Panel { FillColor = Color.FromArgb(26, 26, 26), Location = new Point(0, 0), Size = new Size(440, 55) };
            top.MouseDown += (s, e) => { _dragging = true; _dragStart = e.Location; };
            top.MouseMove += (s, e) => { if (_dragging) Location = new Point(Location.X + e.X - _dragStart.X, Location.Y + e.Y - _dragStart.Y); };
            top.MouseUp   += (s, e) => _dragging = false;
            top.Controls.Add(new Label { Text = title, Font = new Font("Segoe UI", 14F, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.Transparent, Location = new Point(20, 12), Size = new Size(350, 30) });
            var close = new Guna2Button { Text = "✕", BorderRadius = 14, FillColor = Color.FromArgb(220, 53, 69), Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.White, Location = new Point(400, 13), Size = new Size(28, 28) };
            close.Click += (s, e) => Close();
            top.Controls.Add(close);
            Controls.Add(top);

            int y = 70;
            if (!_isFree) AddField("PERSON", cmbPerson, ref y);
            AddField("NAME", txtName, ref y);
            AddField("PHONE", txtPhone, ref y);
            AddField("COACH / SESSION", cmbCoach, ref y);
            if (!_isFree)
            {
                AddField("AMOUNT (EGP)", numAmount, ref y);
                cmbMethod.Items.AddRange(new object[] { "Cash", "Vodafone Cash", "InstaPay" });
                cmbMethod.SelectedIndex = 0;
                AddField("PAID BY", cmbMethod, ref y);
            }

            var save = new Guna2Button
            {
                Text         = _isFree ? "Record Free Session" : "Record Payment",
                BorderRadius = 8,
                FillColor    = _isFree ? Color.FromArgb(40, 167, 69) : Color.FromArgb(242, 101, 34),
                Font         = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor    = Color.White,
                Location     = new Point(30, y + 8),
                Size         = new Size(380, 44)
            };
            save.Click += btnSave_Click;
            Controls.Add(save);
            ClientSize = new Size(440, y + 72);
        }

        private void AddField(string label, Control input, ref int y)
        {
            Controls.Add(new Label { Text = label, Font = new Font("Segoe UI", 8F, FontStyle.Bold), ForeColor = Color.FromArgb(100, 100, 100), Location = new Point(30, y), Size = new Size(380, 17) });
            input.Location = new Point(30, y + 20);
            input.Width    = 380;
            Controls.Add(input);
            y += 70;
        }

        private static ComboBox NewCombo() => new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font          = new Font("Segoe UI", 10F),
            Size          = new Size(380, 30)
        };

        private static Guna2TextBox NewTextBox() => new Guna2TextBox
        {
            BorderRadius = 8,
            DefaultText  = "",
            Font         = new Font("Segoe UI", 10F),
            Size         = new Size(380, 38)
        };

        private void LoadData()
        {
            try
            {
                cmbCoach.DisplayMember = "Value";
                cmbCoach.ValueMember   = "Key";
                cmbCoach.DataSource    = DatabaseHelper.GetAllTrainers()
                    .Select(t => new KeyValuePair<int, string>(t.TrainerID,
                        string.IsNullOrWhiteSpace(t.Specialty) ? t.Name : $"{t.Name} ({t.Specialty})"))
                    .ToList();

                if (_isFree) return;

                var people = new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(0, "New person") };
                people.AddRange(DatabaseHelper.GetAllMembers().Select(m =>
                    new KeyValuePair<int, string>(m.MemberID, $"{m.Name}  -  {m.Phone}")));
                cmbPerson.DisplayMember = "Value";
                cmbPerson.ValueMember   = "Key";
                cmbPerson.DataSource    = people;
                cmbPerson.SelectedIndexChanged += (s, e) => ShowPerson();
                cmbCoach.SelectedIndexChanged  += (s, e) => ShowLastPrice();
                ShowLastPrice();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private int SelectedPersonID => _isFree || cmbPerson.SelectedValue == null ? 0 : (int)cmbPerson.SelectedValue;

        // Name and phone are typed only for someone who isn't in the list yet.
        private void ShowPerson()
        {
            bool isNew = SelectedPersonID == 0;
            txtName.Enabled  = isNew;
            txtPhone.Enabled = isNew;
            var m = isNew ? null : DatabaseHelper.GetMemberById(SelectedPersonID);
            txtName.Text  = m?.Name ?? "";
            txtPhone.Text = m?.Phone ?? "";
        }

        private void ShowLastPrice()
        {
            if (cmbCoach.SelectedValue is int coachID)
                numAmount.Value = Math.Min(DatabaseHelper.GetSingleSessionPrice(coachID), numAmount.Maximum);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!(cmbCoach.SelectedValue is int coachID))
            {
                MessageBox.Show("Please add a coach first (Coaches screen).");
                return;
            }

            int personID = SelectedPersonID;
            string name  = txtName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            if (personID == 0 && name.Length == 0)
            {
                MessageBox.Show("Please enter the person's name.");
                return;
            }
            if (_isFree && phone.Length == 0)
            {
                MessageBox.Show("A phone number is required, because each person gets only one free session.");
                return;
            }
            if (!_isFree && numAmount.Value <= 0)
            {
                MessageBox.Show("Please enter the amount paid.");
                return;
            }

            string coach = ((KeyValuePair<int, string>)cmbCoach.SelectedItem).Value;
            try
            {
                int id = _isFree
                    ? DatabaseHelper.RecordFreeSession(name, phone, coachID)
                    : DatabaseHelper.RecordPaidSession(personID, name, phone, coachID, numAmount.Value, cmbMethod.SelectedItem.ToString());

                MessageBox.Show(
                    _isFree
                        ? $"Free first session recorded for {name} with {coach}.\n\nMember ID: {id}"
                        : $"{name} paid {numAmount.Value:N0} EGP for one session.\n\nThe money goes to {coach}.\nMember ID: {id}",
                    "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Cannot Record", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }
    }
}
