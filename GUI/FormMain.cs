using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ProjectTaskManagement.BUS;
using ProjectTaskManagement.DTO;

namespace ProjectTaskManagement.GUI
{
    public partial class FormMain : Form
    {
        private XeBUS xeBUS = new XeBUS();
        private PhieuSuaChuaBUS phieuBUS = new PhieuSuaChuaBUS();

        private TextBox txtBienSo;
        private TextBox txtTenKH;
        private TextBox txtDienThoai;
        private TextBox txtDiaChi;
        private TextBox txtHieuXe;
        private TextBox txtNamSX;
        private TextBox txtNoiDung;
        private Button btnTiepNhan;
        private Button btnLapPhieu;
        private ListBox lstConsole;

        public FormMain()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Hệ thống Quản lý Garage Ô tô (ProjectTaskManagement)";
            this.Size = new System.Drawing.Size(700, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblBienSo = new Label() { Text = "Biển số:", Left = 20, Top = 20, Width = 80 };
            txtBienSo = new TextBox() { Left = 110, Top = 20, Width = 150, Text = "30A-888.88" };

            Label lblTenKH = new Label() { Text = "Tên KH:", Left = 20, Top = 50, Width = 80 };
            txtTenKH = new TextBox() { Left = 110, Top = 50, Width = 150, Text = "Nguyễn Văn B" };

            Label lblSdt = new Label() { Text = "Điện thoại:", Left = 20, Top = 80, Width = 80 };
            txtDienThoai = new TextBox() { Left = 110, Top = 80, Width = 150, Text = "0912345678" };

            Label lblDiaChi = new Label() { Text = "Địa chỉ:", Left = 280, Top = 20, Width = 80 };
            txtDiaChi = new TextBox() { Left = 360, Top = 20, Width = 150, Text = "Hà Nội" };

            Label lblHieuXe = new Label() { Text = "Hiệu xe:", Left = 280, Top = 50, Width = 80 };
            txtHieuXe = new TextBox() { Left = 360, Top = 50, Width = 150, Text = "Honda CRV" };

            Label lblNamSX = new Label() { Text = "Năm SX:", Left = 280, Top = 80, Width = 80 };
            txtNamSX = new TextBox() { Left = 360, Top = 80, Width = 150, Text = "2023" };

            Label lblNoiDung = new Label() { Text = "Nội dung sửa:", Left = 20, Top = 120, Width = 80 };
            txtNoiDung = new TextBox() { Left = 110, Top = 120, Width = 400, Text = "Bảo dưỡng định kỳ & Thay dầu" };

            btnTiepNhan = new Button() { Text = "1. Tiếp nhận xe", Left = 110, Top = 160, Width = 150, Height = 35 };
            btnTiepNhan.Click += BtnTiepNhan_Click;

            btnLapPhieu = new Button() { Text = "2. Lập phiếu & Thanh toán", Left = 280, Top = 160, Width = 180, Height = 35 };
            btnLapPhieu.Click += BtnLapPhieu_Click;

            lstConsole = new ListBox() { Left = 20, Top = 210, Width = 640, Height = 220 };

            this.Controls.Add(lblBienSo); this.Controls.Add(txtBienSo);
            this.Controls.Add(lblTenKH); this.Controls.Add(txtTenKH);
            this.Controls.Add(lblSdt); this.Controls.Add(txtDienThoai);
            this.Controls.Add(lblDiaChi); this.Controls.Add(txtDiaChi);
            this.Controls.Add(lblHieuXe); this.Controls.Add(txtHieuXe);
            this.Controls.Add(lblNamSX); this.Controls.Add(txtNamSX);
            this.Controls.Add(lblNoiDung); this.Controls.Add(txtNoiDung);
            this.Controls.Add(btnTiepNhan);
            this.Controls.Add(btnLapPhieu);
            this.Controls.Add(lstConsole);
        }

        private void Log(string msg)
        {
            lstConsole.Items.Add($"[{DateTime.Now:HH:mm:ss}] {msg}");
        }

        private void BtnTiepNhan_Click(object sender, EventArgs e)
        {
            string bienSo = txtBienSo.Text.Trim();
            if (xeBUS.KiemTraXeTonTai(bienSo))
            {
                Log($"Xe biển số '{bienSo}' đã tồn tại trong CSDL.");
                MessageBox.Show("Xe đã tồn tại trong hệ thống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var kh = new KhachHangDTO { TenKH = txtTenKH.Text.Trim(), DienThoai = txtDienThoai.Text.Trim(), DiaChi = txtDiaChi.Text.Trim() };
            var xe = new XeDTO { BienSo = bienSo, HieuXe = txtHieuXe.Text.Trim(), NamSanXuat = int.TryParse(txtNamSX.Text, out int n) ? n : 2023 };

            if (xeBUS.TiepNhanXeMoi(kh, xe))
            {
                Log($"Thêm khách hàng '{kh.TenKH}' và xe '{xe.BienSo}' thành công!");
                MessageBox.Show("Tiếp nhận xe mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnLapPhieu_Click(object sender, EventArgs e)
        {
            string bienSo = txtBienSo.Text.Trim();

            var dsChiTiet = new List<ChiTietSuaChuaDTO>
            {
                new ChiTietSuaChuaDTO { MaPhuTung = 1, SoLuong = 1, TienCong = 100000 },
                new ChiTietSuaChuaDTO { MaPhuTung = 3, SoLuong = 1, TienCong = 150000 }
            };

           Log("Đang chạy SqlTransaction lập phiếu sửa chữa & trừ tồn kho...");
if (phieuBUS.LapPhieuSuaChua(bienSo, txtNoiDung.Text.Trim(), dsChiTiet, out int maPhieu, out string errorMsg))
{
    Log($"Lập phiếu #{maPhieu} thành công và đã trừ tồn kho!");

    decimal tongTien = phieuBUS.ThanhToan(maPhieu);
    Log($"Thanh toán hoàn tất phiếu #{maPhieu}. Tổng tiền: {tongTien:N0} VNĐ");

    MessageBox.Show($"Thanh toán thành công phiếu #{maPhieu}!\nTổng tiền: {tongTien:N0} VNĐ", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
else
{
    Log($"Thất bại (Transaction Rollback): {errorMsg}");
    MessageBox.Show($"Lỗi: {errorMsg}", "Lỗi Transaction", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
        }
    }
}
