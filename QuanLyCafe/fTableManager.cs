using QuanLyCafe.DAO;
using QuanLyCafe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyCafe
{
    public partial class fTableManager : Form
    {
        private Account loginAccount;

        public Account LoginAccount
        {
            get { return loginAccount; }
            set { loginAccount = value; ChangeAccount(loginAccount.Type); }
        }

        public fTableManager(Account acc)
        {
            InitializeComponent();
            this.LoginAccount = acc;
            LoadTable();
            LoadCategory();
            LoadComboboxTable(cbSwitchTable);
        }
        #region Method

        private void CreateReceipt(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics graphic = e.Graphics;
            Font font = new Font("Courier New", 12);
            float FontHeight = font.GetHeight();
            int startX = 10;
            int startY = 10;
            int offset = 40;
            int id = (lsvBill.Tag as Table).ID;
            int discount = (int)nmDisCount.Value;
            Table table = lsvBill.Tag as Table;
            graphic.DrawString("\nQUÁN CÀ PHÊ BEST YASUA", new Font("Courier New", 18, FontStyle.Bold), new SolidBrush(Color.Black), 250 , startY);
            offset = offset + (int)FontHeight; //make the spacing consistent
            graphic.DrawString("Hóa đơn tính tiền " + (lsvBill.Tag as Table).Name  , font, new SolidBrush(Color.Black), startX + 250, startY + offset);
            offset = offset + (int)FontHeight;
            graphic.DrawString("--------------------------------------------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + (int)FontHeight + 5; //make the spacing consistent
            string top = "Tên món".PadRight(25) + "Số lượng".PadRight(17) + "Đơn giá (VND)".PadRight(16) + "Thành tiền (VND)";
            graphic.DrawString(top, font, new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + (int)FontHeight; //make the spacing consistent
           
           
            float totalPrice = 0;
            DateTime date = new DateTime();
            date = DateTime.Now;
            
            
            
            List<Calculator> listBillInfo = CalculatorDAO.Instance.GetListMenuByTable(id);
            foreach (Calculator item in listBillInfo)
            {
                ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.TotalPrice.ToString());
                totalPrice += item.TotalPrice;

                lsvBill.Items.Add(lsvItem);
                graphic.DrawString(item.FoodName.ToString(), font, new SolidBrush(Color.Black), startX, startY + offset);
                graphic.DrawString(item.Count.ToString(), font, new SolidBrush(Color.Black), startX + 300, startY + offset);
                graphic.DrawString(item.Price.ToString(), font, new SolidBrush(Color.Black), startX + 450, startY + offset);
                graphic.DrawString(item.TotalPrice.ToString(), font, new SolidBrush(Color.Black), startX + 650, startY + offset);
                
                offset = offset + (int)FontHeight + 5; //make the spacing consistent              
            }
            offset = offset + (int)FontHeight; //make the spacing consistent
            graphic.DrawString("Giảm giá".PadLeft(58), font, new SolidBrush(Color.Black), startX , startY + offset);
            graphic.DrawString(discount.ToString() +"%", font, new SolidBrush(Color.Black), startX + 650, startY + offset);
            offset = offset + (int)FontHeight;
            graphic.DrawString("--------------------------------------------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + 20; //make some room so that the total stands out.
            offset = offset + (int)FontHeight; //make the spacing consistent
            graphic.DrawString("TỔNG TIỀN TRẢ ".PadLeft(58), new Font("Courier New", 12, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + offset);
            graphic.DrawString((totalPrice-(totalPrice * discount /100)).ToString(), new Font("Courier New", 12, FontStyle.Bold), new SolidBrush(Color.Black), startX + 650, startY + offset);

            offset = offset + (int)FontHeight; //make the spacing consistent
            graphic.DrawString("Thời gian thanh toán".PadLeft(55), font, new SolidBrush(Color.Black), startX, startY + offset);
            graphic.DrawString(date.ToString(), font, new SolidBrush(Color.Black), startX + 600, startY + offset);
            offset = offset + (int)FontHeight + 5;           
            graphic.DrawString(" CẢM ƠN BẠN ĐÃ GHÉ THĂM!", font, new SolidBrush(Color.Black), startX+250, startY + offset);
            offset = offset + (int)FontHeight + 5;               
            graphic.DrawString("HI VỌNG BẠN SẼ GHÉ THĂM LẠI!", font, new SolidBrush(Color.Black), startX + 250, startY + offset);
        }

        void ChangeAccount(int type)
        {

            adminToolStripMenuItem.Enabled = type == 1;
            thôngTinTàiKhoảnToolStripMenuItem.Text += " (" + LoginAccount.DisplayName + ")";
        }

        void LoadCategory()
        {
            List<Category> listCaterory = CategoryDAO.Instance.GetListCategory();
            cbCategory.DataSource = listCaterory;
            cbCategory.DisplayMember = "Name";
        }

        void LoadFoodListByCategoryID(int id)
        {

            List<Food> listFood = FoodDAO.Instance.GetFoodByCategoryID(id);
            cbFood.DataSource = listFood;
            cbFood.DisplayMember = "Name";
        }

        void LoadTable()
        {
            flpTable.Controls.Clear();
            List<Table> tableList = TableDAO.Instance.LoadTableList();

            foreach (Table item in tableList)
            {
                Button btn = new Button() { Width = TableDAO.TableWidth, Height = TableDAO.TableWidth };
                btn.Text = item.Name + Environment.NewLine + item.Status;
                btn.Click += btn_Click;
                btn.Tag = item;
                switch (item.Status)
                {
                    case "Trống":
                        btn.BackColor = Color.Aqua; break;
                    default:
                        btn.BackColor = Color.Green; break;
                }

                flpTable.Controls.Add(btn);
            }
        }

        void ShowBill(int id)
        {
            lsvBill.Items.Clear();
            List<Calculator> listBillInfo = CalculatorDAO.Instance.GetListMenuByTable(id);

            float totalPrice = 0;
            foreach(Calculator item in listBillInfo)
            {
                ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.TotalPrice.ToString());
                totalPrice += item.TotalPrice;

                lsvBill.Items.Add(lsvItem);
            }
            //CultureInfo culture = new CultureInfo("vi-VN");
           // Thread.CurrentThread.CurrentCulture = culture;
            //txbTotalPrice.Text = totalPrice.ToString("c",culture);
            txbTotalPrice.Text = totalPrice.ToString();

        }
        void LoadComboboxTable(ComboBox cb)
        {
            cb.DataSource = TableDAO.Instance.LoadTableList();
            cb.DisplayMember = "Name";
        }
        #endregion 

        #region Events
        void btn_Click(object sender, EventArgs e)
        {
            int tableID = ((sender as Button).Tag as Table).ID;
            lsvBill.Tag = (sender as Button).Tag ;
            ShowBill(tableID);
        }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có thật sự muốn đăng xuất?", "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                this.Close();
            }
        }

        private void thôngTinCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAccountProfile f = new fAccountProfile(LoginAccount);
            f.UpdateAccount += f_UpdateAccount;
            f.ShowDialog();
        }

        void f_UpdateAccount(object sender, AccountEvent e)
        {
            thôngTinTàiKhoảnToolStripMenuItem.Text = "Thông tin tài khoản (" + e.Acc.DisplayName + ")";
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAdmin f = new fAdmin();
            f.loginAccount = LoginAccount;
            f.InsertFood += f_InsertFood;
            f.DeleteFood += f_DeleteFood;
            f.UpdateFood += f_UpdateFood;
            f.ShowDialog();
        }

        void f_UpdateFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }

        void f_DeleteFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
            LoadTable();
        }

        void f_InsertFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedItem == null)
                return;
            Category selected = cb.SelectedItem as Category;
            id = selected.ID;
            LoadFoodListByCategoryID(id);
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {

            Table table = lsvBill.Tag as Table;
            if(table == null)
            {
                MessageBox.Show("Hãy chọn bàn");
                return;
            }
            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int foodID = (cbFood.SelectedItem as Food).ID ;
            int count = (int)nmFoodCount.Value;
            if(idBill == -1)
            {
                BillDAO.Instance.InsertBill(table.ID);
                BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxIDBill(), foodID, count);
            }
            else
            {
                BillInfoDAO.Instance.InsertBillInfo(idBill, foodID, count);
            }

            ShowBill(table.ID);
            LoadTable();
        }
        
        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;
            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int discount = (int)nmDisCount.Value;
            double totalPrice = Convert.ToDouble(txbTotalPrice.Text.Split(',')[ 0]);
            double finalPrice = totalPrice - (totalPrice * discount / 100);
            if(idBill != -1)
            {
                if (MessageBox.Show(string.Format("Bạn có chắc thanh toán hóa đơn cho bàn {0}\n Tổng tiền - Tổng tiền x Giảm giá / 100 = {1} - {1} * {2} / 100 = {3}",  table.Name, totalPrice,discount,finalPrice) ,"Thông báo", MessageBoxButtons.OKCancel ) == System.Windows.Forms.DialogResult.OK)
                {
                    if (MessageBox.Show(string.Format("Bạn có muốn in hóa đơn không?"), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                    {
                        PrintDialog _PrintDialog = new PrintDialog();
                        PrintDocument _PrintDocument = new PrintDocument();

                        _PrintDialog.Document = _PrintDocument; //add the document to the dialog box

                        _PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(CreateReceipt); //add an event handler that will do the printing
                                                                                                                       //on a till you will not want to ask the user where to print but this is fine for the test envoironment.
                        DialogResult result = _PrintDialog.ShowDialog();

                        if (result == DialogResult.OK)
                        {
                            _PrintDocument.Print();
                        }
                    }

                    BillDAO.Instance.CheckOut(idBill,discount, (float)finalPrice);
                    ShowBill(table.ID);

                    LoadTable();
                }
            }
        }
        

        private void btnSwitchTable_Click(object sender, EventArgs e)
        {

            int id1 = (lsvBill.Tag as Table).ID;

            int id2 = (cbSwitchTable.SelectedItem as Table).ID;
            if (MessageBox.Show(string.Format("Bạn có thật sự muốn chuyển bàn {0} qua bàn {1}", (lsvBill.Tag as Table).Name, (cbSwitchTable.SelectedItem as Table).Name), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                TableDAO.Instance.SwitchTable(id1, id2);

                LoadTable();
            }
        }
        #endregion

        private void thêmMónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnAddFood_Click(this, new EventArgs());
        }

        private void thanhToánToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnCheckOut_Click(this, new EventArgs());
        }
    }
}
