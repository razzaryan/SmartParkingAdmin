#nullable disable
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace SmartParkingAdmin
{
    public partial class Form1 : Form
    {
        private const decimal TARIF_PER_JAM = 15000m;
        private readonly CultureInfo _cultureId = new CultureInfo("id-ID");

        // Komponen Input
        private TextBox _txtJamMasuk;
        private TextBox _txtJamKeluar;

        // Tombol Aksi
        private Button _btnTambah;
        private Button _btnHapusPilihan;
        private Button _btnHapusTerakhir;

        // Opsi & Slot
        private RadioButton _rbSlot1, _rbSlot2, _rbSlot3, _rbSlot4;
        private CheckBox _chkProRata;
        private CheckBox _chkTampilkanStruk;

        // Tabel & Total
        private DataGridView _dgvData;
        private Label _lblTotalPendapatan;
        private decimal _totalPendapatan = 0m;

        public Form1()
        {
            InitializeComponent();
            SetupNeatUI();
        }

        private void SetupNeatUI()
        {
            // Konfigurasi Ukuran Form
            this.Text = "Smart Parking System";
            this.ClientSize = new Size(555, 485);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = SystemColors.Control;

            // ==================== 1. INPUT JAM MASUK & KELUAR ====================
            Label lblMasuk = new Label
            {
                Text = "Jam Masuk :",
                Location = new Point(20, 25),
                AutoSize = true
            };

            // TextBox digeser ke kanan (X = 175) agar tidak bertabrakan dengan label
            _txtJamMasuk = new TextBox
            {
                Location = new Point(175, 22),
                Size = new Size(155, 23),
                Text = "08:00"
            };

            Label lblKeluar = new Label
            {
                Text = "Jam Keluar :",
                Location = new Point(20, 62),
                AutoSize = true
            };

            _txtJamKeluar = new TextBox
            {
                Location = new Point(175, 59),
                Size = new Size(155, 23),
                Text = "10:30"
            };

            // ==================== 2. TOMBOL AKSI (KANAN ATAS) ====================
            _btnTambah = new Button
            {
                Text = "Add",
                Location = new Point(350, 18),
                Size = new Size(185, 32),
                UseVisualStyleBackColor = true
            };
            _btnTambah.Click += BtnTambah_Click;

            _btnHapusPilihan = new Button
            {
                Text = "Hapus Tabel",
                Location = new Point(350, 56),
                Size = new Size(185, 32),
                UseVisualStyleBackColor = true
            };
            _btnHapusPilihan.Click += BtnHapusPilihan_Click;

            _btnHapusTerakhir = new Button
            {
                Text = "Hapus Baris Terakhir",
                Location = new Point(350, 94),
                Size = new Size(185, 32),
                UseVisualStyleBackColor = true
            };
            _btnHapusTerakhir.Click += BtnHapusTerakhir_Click;

            // ==================== 3. GROUPBOX OPSI & SLOT (TENGAH) ====================
            GroupBox grpOpsi = new GroupBox
            {
                Text = "Opsi Tarif",
                Location = new Point(20, 102),
                Size = new Size(310, 115)
            };

            _chkProRata = new CheckBox
            {
                Text = "Tarif Pro-rata (Menit berjalan)",
                Location = new Point(15, 25),
                AutoSize = true
            };

            _chkTampilkanStruk = new CheckBox
            {
                Text = "Tampilkan MessageBox Struk",
                Location = new Point(15, 52),
                AutoSize = true,
                Checked = true
            };

            Label lblInfoTarif = new Label
            {
                Text = "Tarif Dasar: Rp 15.000 / jam",
                Location = new Point(15, 82),
                AutoSize = true,
                ForeColor = SystemColors.ControlDarkDark
            };

            grpOpsi.Controls.Add(_chkProRata);
            grpOpsi.Controls.Add(_chkTampilkanStruk);
            grpOpsi.Controls.Add(lblInfoTarif);

            GroupBox grpSlot = new GroupBox
            {
                Text = "Pilihan Slot Parkir",
                Location = new Point(350, 138),
                Size = new Size(185, 95)
            };

            _rbSlot1 = new RadioButton { Text = "Slot 1", Location = new Point(15, 25), AutoSize = true, Checked = true };
            _rbSlot2 = new RadioButton { Text = "Slot 2", Location = new Point(15, 55), AutoSize = true };
            _rbSlot3 = new RadioButton { Text = "Slot 3", Location = new Point(105, 25), AutoSize = true };
            _rbSlot4 = new RadioButton { Text = "Slot 4", Location = new Point(105, 55), AutoSize = true };

            grpSlot.Controls.Add(_rbSlot1);
            grpSlot.Controls.Add(_rbSlot2);
            grpSlot.Controls.Add(_rbSlot3);
            grpSlot.Controls.Add(_rbSlot4);

            // ==================== 4. TABEL DATA (BAWAH) ====================
            _dgvData = new DataGridView
            {
                Location = new Point(20, 248),
                Size = new Size(515, 175),
                BackgroundColor = SystemColors.Window,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            _dgvData.Columns.Add("ColSlot", "Slot");
            _dgvData.Columns.Add("ColMasuk", "Jam Masuk");
            _dgvData.Columns.Add("ColKeluar", "Jam Keluar");
            _dgvData.Columns.Add("ColDurasi", "Durasi");
            _dgvData.Columns.Add("ColTarif", "Total Tarif");
            _dgvData.Columns.Add("ColRawTarif", "RawTarif");
            _dgvData.Columns["ColRawTarif"].Visible = false;

            _lblTotalPendapatan = new Label
            {
                Text = "Total Pendapatan: Rp 0",
                Font = new Font(this.Font, FontStyle.Bold),
                Location = new Point(20, 438),
                AutoSize = true
            };

            // Tambahkan komponen ke Form
            this.Controls.Add(lblMasuk);
            this.Controls.Add(_txtJamMasuk);
            this.Controls.Add(lblKeluar);
            this.Controls.Add(_txtJamKeluar);
            this.Controls.Add(_btnTambah);
            this.Controls.Add(_btnHapusPilihan);
            this.Controls.Add(_btnHapusTerakhir);
            this.Controls.Add(grpOpsi);
            this.Controls.Add(grpSlot);
            this.Controls.Add(_dgvData);
            this.Controls.Add(_lblTotalPendapatan);
        }

        // ==================== LOGIKA PERHITUNGAN ====================
        private void BtnTambah_Click(object sender, EventArgs e)
        {
            if (!TimeSpan.TryParse(_txtJamMasuk.Text.Trim(), out TimeSpan jamMasuk))
            {
                MessageBox.Show("Format Jam Masuk tidak valid! Gunakan format HH:mm (contoh: 08:00).", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtJamMasuk.Focus();
                return;
            }

            if (!TimeSpan.TryParse(_txtJamKeluar.Text.Trim(), out TimeSpan jamKeluar))
            {
                MessageBox.Show("Format Jam Keluar tidak valid! Gunakan format HH:mm (contoh: 10:30).", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtJamKeluar.Focus();
                return;
            }

            TimeSpan durasi = jamKeluar - jamMasuk;
            if (durasi.TotalMinutes < 0)
            {
                durasi = durasi.Add(TimeSpan.FromHours(24));
            }

            if (durasi.TotalMinutes == 0)
            {
                MessageBox.Show("Jam Keluar tidak boleh sama persis dengan Jam Masuk!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal totalTarif;
            if (_chkProRata.Checked)
            {
                totalTarif = Math.Round((decimal)durasi.TotalHours * TARIF_PER_JAM, 0, MidpointRounding.AwayFromZero);
            }
            else
            {
                double jamBulat = Math.Ceiling(durasi.TotalHours);
                totalTarif = (decimal)jamBulat * TARIF_PER_JAM;
            }

            string slotTerpilih = "Slot 1";
            if (_rbSlot2.Checked) slotTerpilih = "Slot 2";
            else if (_rbSlot3.Checked) slotTerpilih = "Slot 3";
            else if (_rbSlot4.Checked) slotTerpilih = "Slot 4";

            string teksDurasi = $"{(int)durasi.TotalHours}j {durasi.Minutes}m";
            string teksTarif = string.Format(_cultureId, "Rp {0:N0}", totalTarif);

            _dgvData.Rows.Add(
                slotTerpilih,
                jamMasuk.ToString(@"hh\:mm"),
                jamKeluar.ToString(@"hh\:mm"),
                teksDurasi,
                teksTarif,
                totalTarif
            );

            UpdateTotalPendapatan();

            if (_chkTampilkanStruk.Checked)
            {
                MessageBox.Show(
                    $"Slot Parkir : {slotTerpilih}\n" +
                    $"Jam Masuk   : {jamMasuk:hh\\:mm}\n" +
                    $"Jam Keluar  : {jamKeluar:hh\\:mm}\n" +
                    $"Durasi      : {teksDurasi}\n" +
                    $"Total Biaya : {teksTarif}",
                    "Struk Pembayaran Parkir",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void BtnHapusPilihan_Click(object sender, EventArgs e)
        {
            if (_dgvData.SelectedRows.Count > 0)
            {
                _dgvData.Rows.RemoveAt(_dgvData.SelectedRows[0].Index);
                UpdateTotalPendapatan();
            }
            else
            {
                MessageBox.Show("Pilih baris pada tabel yang ingin dihapus terlebih dahulu.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnHapusTerakhir_Click(object sender, EventArgs e)
        {
            int jumlahBaris = _dgvData.Rows.Count;
            if (jumlahBaris > 0)
            {
                _dgvData.Rows.RemoveAt(jumlahBaris - 1);
                UpdateTotalPendapatan();
            }
            else
            {
                MessageBox.Show("Tabel data masih kosong.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UpdateTotalPendapatan()
        {
            _totalPendapatan = 0m;
            foreach (DataGridViewRow row in _dgvData.Rows)
            {
                if (row.Cells["ColRawTarif"].Value is decimal nilai)
                {
                    _totalPendapatan += nilai;
                }
            }
            _lblTotalPendapatan.Text = string.Format(_cultureId, "Total Pendapatan: Rp {0:N0}", _totalPendapatan);
        }
    }
}