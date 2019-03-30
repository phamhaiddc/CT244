using QuanLyCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe.DAO
{
    public class CalculatorDAO
    {
       
        private static CalculatorDAO instance;

        public static CalculatorDAO Instance
        {
            get { if (instance == null) instance = new CalculatorDAO(); return CalculatorDAO.instance; }
            private set { CalculatorDAO.instance = value; }
        }

        private CalculatorDAO() { }

        public List<Calculator> GetListMenuByTable(int id)
        {
            List<Calculator> listMenu = new List<Calculator>();

            string query = "SELECT f.name, bi.count, f.price, f.price*bi.count AS totalPrice FROM dbo.BillInfo AS bi, dbo.Bill AS b, dbo.Food AS f WHERE bi.idBill = b.id AND bi.idFood = f.id AND b.status = 0 AND b.status = 0 AND b.idTable = " + id;
            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                Calculator cal = new Calculator(item);
                listMenu.Add(cal);
            }

            
            return listMenu;
        }
    }
}
