﻿
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLVT_DATHANG
{
    public partial class frmNhanVien : Form
    {
        private int vitri;
        private string macn;
        private Stack<String> stackundo = new Stack<string>(16);
        String query = "";
        public frmNhanVien()
        {
            InitializeComponent();
        }

        private void nhanVienBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.nhanVienBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.dataSet);

        }

        private void frmNhanVien_Load(object sender, EventArgs e)
        {
            LoadTable();
            cmbCN.DataSource = Program.bds_dspm.DataSource;
            cmbCN.DisplayMember = "TENCN";
            cmbCN.ValueMember = "TENSERVER";
            cmbCN.SelectedIndex = Program.mChinhanh;

        }

        private void tENLabel_Click(object sender, EventArgs e)
        {

        }

        private void LoadTable()
        {
            try
            {
                this.dataSet.EnforceConstraints = false;
                this.chiNhanhTableAdapter.Connection.ConnectionString = Program.connstr;
                this.chiNhanhTableAdapter.Fill(this.dataSet.ChiNhanh);

                this.nhanVienTableAdapter.Connection.ConnectionString = Program.connstr;
                this.nhanVienTableAdapter.Fill(this.dataSet.NhanVien);

                this.datHangTableAdapter.Connection.ConnectionString = Program.connstr;
                this.datHangTableAdapter.Fill(this.dataSet.DatHang);

                this.phieuNhapTableAdapter.Connection.ConnectionString = Program.connstr;
                this.phieuNhapTableAdapter.Fill(this.dataSet.PhieuNhap);

                this.phieuXuatTableAdapter.Connection.ConnectionString = Program.connstr;
                this.phieuXuatTableAdapter.Fill(this.dataSet.PhieuXuat);


                macn = ((DataRowView)nhanVienBindingSource[0])["MACN"].ToString();
                if (Program.mGroup.Equals("CONGTY"))
                {
                    btnThem.Enabled = btnXoa.Enabled = btnSua.Enabled = btnReload.Enabled = false;
                    btnGhi.Enabled = btnUndo.Enabled = btnCCN.Enabled = false;
                    cmbCN.Enabled = true;
                    groupBox1.Enabled = false;
                }
                else if (Program.mGroup == "USSER")
                {
                    btnXoa.Enabled = btnSua.Enabled = btnReload.Enabled = true;
                    btnThem.Enabled = true;
                    cmbCN.Enabled = txtCN.Enabled = false;
                    btnGhi.Enabled = btnUndo.Enabled = false;
                    btnCCN.Enabled = false;
                    groupBox1.Enabled = false;
                }
                else if (Program.mGroup == "CHINHANH")
                {
                    btnThem.Enabled = btnXoa.Enabled = btnSua.Enabled = btnReload.Enabled
                        = true;
                    btnGhi.Enabled = btnUndo.Enabled = false;
                    btnCCN.Enabled = true;
                    cmbCN.Enabled = false; txtCN.Enabled = false;
                    groupBox1.Enabled = false;
                }
                //nhanVienGridControl.Enabled = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void loadUndo()
        {
            if (stackundo.Count != 0)
            {
                btnUndo.Enabled = true;
            }
            else btnUndo.Enabled = false;
        }
        private void cmbCN_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCN.SelectedValue.ToString() == "System.Data.DataRowView")
                return;

            Program.servername = cmbCN.SelectedValue.ToString();

            if (cmbCN.SelectedIndex != Program.mChinhanh)
            {
                Program.mlogin = Program.remotelogin;
                Program.password = Program.remotepassword;
            }
            else
            {
                Program.mlogin = Program.mloginDN;
                Program.password = Program.passwordDN;
            }

            if (Program.KetNoi() == 0)
                MessageBox.Show("Lỗi kết nối về chi nhánh mới", string.Empty, MessageBoxButtons.OK);
            else
            {
                LoadTable();
            }
        }
        private int TaoMaNV()
        {
            int maxMaNV = 0;
            //LINK2  kết nối từ site 1 đến site 3 hoặc từ 2 đến 3
            //string lenh2= string.Format("SELECT MAX(MANV) AS MAXNV FROM LINK2.QLVT_DATHANG.dbo.NhanVien");
            string lenh2 = string.Format("SELECT MAX(MANV) FROM NhanVien");
            string lenh3 = string.Format("SELECT MAX(MANV) FROM LINK1.QLVT_DATHANG.dbo.NhanVien");
            using (SqlConnection connection = new SqlConnection(Program.connstr))
            {
                connection.Open();
                SqlCommand sqlcmt = new SqlCommand(lenh2, connection);
                SqlCommand sqlcmt1 = new SqlCommand(lenh3, connection);
                sqlcmt.CommandType = CommandType.Text;
                try
                {
                    int a = (Int32)sqlcmt.ExecuteScalar();
                    int b = (Int32)sqlcmt1.ExecuteScalar();
                    maxMaNV = (a > b ? a : b);

                    //maxMaNV = (Int32)sqlcmt.ExecuteScalar();
                }
                catch { }
            }
            return (maxMaNV + 1);
        }

        private void DisableForm()
        {
            btnThem.Enabled = btnSua.Enabled = btnXoa.Enabled = btnReload.Enabled = false;
            btnGhi.Enabled = btnThoat.Enabled = true;
            txtCN.Enabled = cmbCN.Enabled = false;
            btnCCN.Enabled = false;
        }
        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txtMaNV.Enabled = false;
            groupBox1.Enabled = true;
            vitri = nhanVienBindingSource.Position;
            nhanVienBindingSource.AddNew();
            txtMaNV.Text = TaoMaNV() + "";
            txtCN.Text = macn;
            txtTTX.Text = "0";
            txtTTX.Enabled = false;
            txtNgaySinh.Text = "1/1/2000";
            query = String.Format("Delete from NhanVien where MANV={0}", txtMaNV.Text);
            btnUndo.Enabled = false;
            DisableForm();
        }

        private void btnGhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txtHo.Text = chuanhoa(txtHo.Text);
            txtTen.Text = chuanhoa(txtTen.Text);
            txtDiaChi.Text = chuanhoa(txtDiaChi.Text);
            if (txtHo.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Họ nhân viên không được thiếu !", string.Empty, MessageBoxButtons.OK);
                txtHo.Focus();
                return;
            }
            if (txtTen.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Tên nhân viên không được thiếu !", string.Empty, MessageBoxButtons.OK);
                txtTen.Focus();
                return;
            }
            if (txtDiaChi.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Địa chỉ không được thiếu !", string.Empty, MessageBoxButtons.OK);
                txtDiaChi.Focus();
                return;
            }
            if (txtNgaySinh.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Ngày sinh không được thiếu !", string.Empty, MessageBoxButtons.OK);
                txtNgaySinh.Focus();
                return;
            }
            if (DateTime.Now.Year-txtNgaySinh.DateTime.Year<18)
            {
                MessageBox.Show("Ngày sinh không hợp lệ !", string.Empty, MessageBoxButtons.OK);
                txtNgaySinh.Focus();
                return;
            }    
            if (txtLuong.Value < 4000000)
            {
                MessageBox.Show("Vui lòng nhập lương lớn hơn 4.000.000", "", MessageBoxButtons.OK);
                txtLuong.Focus();
                return;
            }
            try
            {
                //Lưu vô DataSet
                nhanVienBindingSource.EndEdit();
                nhanVienBindingSource.ResetCurrentItem();

                //Lưu vô CSDl
                this.nhanVienTableAdapter.Connection.ConnectionString = Program.connstr;
                this.nhanVienTableAdapter.Update(this.dataSet.NhanVien);
                MessageBox.Show("Ghi thành công !", "", MessageBoxButtons.OK);
                stackundo.Push(query);
                LoadTable();
                loadUndo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ghi nhân viên.\n" + ex.Message);
                return;
            }
            //LoadTable();
        }

        private void btnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //nut = "SUA";
            groupBox1.Enabled = true;
            vitri = nhanVienBindingSource.Position;
            txtMaNV.Enabled = false;
            txtTTX.Enabled = false;
            btnUndo.Enabled = false;
            DisableForm();
            query = String.Format("Update NhanVien Set HO=N'{1}',TEN=N'{2}',DIACHI=N'{3}',NGAYSINH=N'{4}',LUONG={5},MACN=N'{6}',TrangThaiXoa={7} where MANV={0}", txtMaNV.Text, txtHo.Text, txtTen.Text, txtDiaChi.Text, txtNgaySinh.Text, txtLuong.Text, txtCN.Text, txtTTX.Text);
          
        }

        private void btnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (groupBox1.Enabled)
            {
                if (MessageBox.Show("Dữ liệu Form Nhân Viên vẫn chưa lưu vào Database! \nBạn có chắn chắn muốn thoát?", "Thông báo",
                            MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int trangthaixoa = int.Parse(((DataRowView)nhanVienBindingSource[nhanVienBindingSource.Position])["TrangThaiXoa"].ToString());
            int manv = 0;
            if (trangthaixoa == 1)
            {
                MessageBox.Show("Nhân viên này đã nghỉ làm hoặc chuyển chi nhánh. Vui lòng chọn nhân viên khác !", "", MessageBoxButtons.OK);
                return;
            }
            else if (phieuNhapBindingSource.Count + phieuXuatBindingSource.Count + datHangBindingSource.Count > 0)
            {
                MessageBox.Show("Không thể xóa nhân viên này vì đã lập phiếu", "", MessageBoxButtons.OK);
                return;
            }
            else if (txtMaNV.Text.Trim() == Program.username)
            {
                MessageBox.Show("Bạn không thể xóa chính mình !", "", MessageBoxButtons.OK);
                return;
            }
            else if (MessageBox.Show("Bạn có thật sự muốn xóa nhân viên này ?", "Xác nhận", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    manv = int.Parse(((DataRowView)nhanVienBindingSource[nhanVienBindingSource.Position])["MANV"].ToString());
                    String ho = ((DataRowView)nhanVienBindingSource[nhanVienBindingSource.Position])["HO"].ToString();
                    String ten = ((DataRowView)nhanVienBindingSource[nhanVienBindingSource.Position])["TEN"].ToString();
                    String diachi = ((DataRowView)nhanVienBindingSource[nhanVienBindingSource.Position])["DIACHI"].ToString();
                    String ngaysinh = ((DataRowView)nhanVienBindingSource[nhanVienBindingSource.Position])["NGAYSINH"].ToString();
                    float luong = float.Parse(((DataRowView)nhanVienBindingSource[nhanVienBindingSource.Position])["LUONG"].ToString());
                    String macn = ((DataRowView)nhanVienBindingSource[nhanVienBindingSource.Position])["MACN"].ToString();
                    String ttx = ((DataRowView)nhanVienBindingSource[nhanVienBindingSource.Position])["TrangThaiXoa"].ToString();
                    //nhanVienBindingSource.RemoveCurrent();
                    //this.nhanVienTableAdapter.Connection.ConnectionString = Program.connstr;
                    //this.nhanVienTableAdapter.Update(this.dataSet.NhanVien);
                   
                    Boolean kiemtra = false;
                    String lenh2 = String.Format("EXEC sp_kiemtranhanviencologin {0} ", manv);
                    using (SqlConnection connection = new SqlConnection(Program.connstr))
                    {
                        connection.Open();
                        SqlCommand sqlcmt = new SqlCommand(lenh2, connection);
                        sqlcmt.CommandType = CommandType.Text;
                        try
                        {
                            SqlDataReader a = sqlcmt.ExecuteReader();
                            
                            while (a.Read())
                            {
                                kiemtra = true;
                            }

                        }
                        catch
                        {
                            MessageBox.Show(lenh2);
                        }
                    }
                    
                    if (kiemtra)
                    {
                        String lenh3 = String.Format("EXEC sp_checkquyenxoanhanvien {0},{1} ", manv, Program.mGroup);
                        using (SqlConnection connection = new SqlConnection(Program.connstr))
                        {
                            connection.Open();
                            SqlCommand sqlcmt = new SqlCommand(lenh3, connection);
                            sqlcmt.CommandType = CommandType.Text;
                            try
                            {
                                //MessageBox.Show(lenh3);
                                sqlcmt.ExecuteNonQuery();
                                //SqlDataReader a = sqlcmt.ExecuteReader();

                                //while (a.Read())
                                //{
                                //    kiemtra = true;
                                //}

                            }
                            catch (SqlException ex)
                            {
                                MessageBox.Show(ex.Message);
                                return;
                            }
                        }
                        String lenh1 = String.Format("EXEC sp_layloginname {0} ", manv);
                        String Loginname = "";
                        using (SqlConnection connection = new SqlConnection(Program.connstr))
                        {
                            connection.Open();
                            SqlCommand sqlcmt = new SqlCommand(lenh1, connection);
                            sqlcmt.CommandType = CommandType.Text;
                            try
                            {
                                Loginname = (string)sqlcmt.ExecuteScalar();

                            }
                            catch
                            {
                                MessageBox.Show(lenh1);
                            }
                        }
                        query = String.Format("EXEC sp_undoxoanhanvien '{0}',{1},N'{2}',N'{3}',N'{4}','{5}', {6},'{7}'", Loginname, manv, ho, ten, diachi, ngaysinh, luong, macn);
                        String lenh = String.Format("EXEC sp_xoatkdangnhap {0} ", manv);
                        using (SqlConnection connection = new SqlConnection(Program.connstr))
                        {
                            connection.Open();
                            SqlCommand sqlcmt = new SqlCommand(lenh, connection);
                            sqlcmt.CommandType = CommandType.Text;
                            try
                            {
                                sqlcmt.ExecuteNonQuery();

                            }
                            catch
                            {
                                MessageBox.Show(lenh);
                            }
                        }
                    }
                    else
                    {
                        query = String.Format("insert into NhanVien (MANV, HO, TEN, DIACHI, NGAYSINH, LUONG, MACN, TrangThaiXoa) Values ({0},N'{1}',N'{2}',N'{3}','{4}',{5},'{6}',0)", manv, ho, ten, diachi, ngaysinh, luong, macn);
                    }
                    nhanVienBindingSource.RemoveCurrent();
                    this.nhanVienTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.nhanVienTableAdapter.Update(this.dataSet.NhanVien);
             
                    stackundo.Push(query);
                    LoadTable();
                    loadUndo();
                    //LoadTable();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa nhân viên. Bạn hãy xóa lại \n", ex.Message, MessageBoxButtons.OK);
                    this.nhanVienTableAdapter.Fill(this.dataSet.NhanVien);
                    nhanVienBindingSource.Position = nhanVienBindingSource.Find("MANV", manv);
                    return;
                }
            }
        }

        private int ChuyenChiNhanh(int MaHT, int MaMoi)
        {
            int result = 1;
            string lenh = string.Format("EXEC SP_ChuyenCN {0}, {1} ", MaHT, MaMoi);
            //string lenh = string.Format("EXEC sp_chuyenchinhanh {0}, {1} ", MaHT, MaMoi);
            using (SqlConnection connection = new SqlConnection(Program.connstr))
            {
                connection.Open();
                SqlCommand sqlcmt = new SqlCommand(lenh, connection);
                sqlcmt.CommandType = CommandType.Text;
                try
                {
                    SqlDataReader a = sqlcmt.ExecuteReader();

                    while (a.Read())
                    {
                        int loaichuyen = a.GetInt32(0);
                        if(loaichuyen == 2)
                        {
                            MaMoi = a.GetInt32(1);
                        }
                        //MessageBox.Show("" + loaichuyen);
                        query = string.Format("EXEC sp_undochuyencn {0}, {1},{2} ", MaHT, MaMoi, loaichuyen);
                    }
                    
                    
                    
                }
                catch
                {
                    result = 0;
                }
                return MaMoi;
            }
        }

        private void btnCCN_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if ((MessageBox.Show("Bạn chắc chắn muốn chuyển nhân viên này ?", "", MessageBoxButtons.OKCancel)) == DialogResult.OK)
            {
                int trangthaixoa = int.Parse(((DataRowView)nhanVienBindingSource[nhanVienBindingSource.Position])["TrangThaiXoa"].ToString());

                if (trangthaixoa == 1)
                {
                    MessageBox.Show("Nhân viên này đã nghỉ làm hoặc chuyển chi nhánh. Vui lòng chọn nhân viên khác !", "", MessageBoxButtons.OK);
                    return;
                }

                try
                {
                    int maNV = int.Parse(((DataRowView)nhanVienBindingSource[nhanVienBindingSource.Position])["MANV"].ToString());
                    int mamoi = ChuyenChiNhanh(maNV, TaoMaNV());
                    MessageBox.Show("Chuyển chi nhánh thành công ! \n Mã nhân viên mới là: " + mamoi, "", MessageBoxButtons.OK);
                    stackundo.Push(query);      
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi chuyển chi nhánh" + ex.Message);
                    return;
                }

            }
            else
            {
                return;
            }
            LoadTable();
            loadUndo();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadTable();
        }

      

        private void btnUndo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            String lenh = stackundo.Pop();
            using (SqlConnection connection = new SqlConnection(Program.connstr))
            {
                connection.Open();
                SqlCommand sqlcmt = new SqlCommand(lenh, connection);
                sqlcmt.CommandType = CommandType.Text;
                try
                {
                    //MessageBox.Show(lenh);
                    sqlcmt.ExecuteNonQuery();
                    LoadTable();
                    loadUndo();
                    //LoadTable();
                }
                catch
                {
                    MessageBox.Show(lenh);
                }
            }
        }
        private String chuanhoa(String a)
        {
            a = a.Trim();
            while(a.Contains("  "))
            {
               a = a.Replace("  ", " ");
            }
            a = new System.Globalization.CultureInfo("en-US", false).TextInfo.ToTitleCase(a.ToLower());
            return a;
        }

        private void txtHo_EditValueChanged(object sender, EventArgs e)
        {

        }
    }
}
