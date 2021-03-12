using System;
using System.Collections.Generic;
using System.Linq;
using Linq.Objects;

namespace Linq
{
    public static class Tasks
    {
        //The implementation of your tasks should look like this:
        public static string TaskExample(IEnumerable<string> stringList)
        {
            return stringList.Aggregate<string>((x, y) => x + y);
        }

        #region Low

        public static IEnumerable<string> Task1(char c, IEnumerable<string> stringList)
        {
            return stringList.Where(str => str.Length > 1 && str.StartsWith(c) && str.EndsWith(c));
        }

        public static IEnumerable<int> Task2(IEnumerable<string> stringList)
        {
            return stringList.Select(str => str.Length).OrderBy(str => str);
        }

        public static IEnumerable<string> Task3(IEnumerable<string> stringList)
        {
            return stringList.Select(str => str.Substring(0, 1) + str.Substring(str.Length - 1, 1));
        }

        public static IEnumerable<string> Task4(int k, IEnumerable<string> stringList)
        {
            return stringList
                .Where(str => str.Length >= k)
                .Select(str => str.Substring(0, k))
                .Where(str => str.Length > 0 && char.IsDigit(str[str.Length - 1]))
                .OrderBy(str => str);
        }

        public static IEnumerable<string> Task5(IEnumerable<int> integerList)
        {
            return integerList
                .Where(e => e % 2 != 0)
                .OrderBy(e => e)
                .Select(e => e.ToString());
        }

        #endregion

        #region Middle

        public static IEnumerable<string> Task6(IEnumerable<int> numbers, IEnumerable<string> stringList)
        {
            return numbers
                .GroupJoin(stringList.Where(str => str.Length > 0 && char.IsDigit(str[0])),
                    n => n,
                    str => str.Length,
                    (x, y) => y.FirstOrDefault())
                .Select(x => x ?? "Not found");
        }

        public static IEnumerable<int> Task7(int k, IEnumerable<int> integerList)
        {
            return integerList
                .Where(e => e % 2 == 0)
                .Except(integerList.Skip(k))
                .Reverse();
        }
        
        public static IEnumerable<int> Task8(int k, int d, IEnumerable<int> integerList)
        {
            return integerList
                .TakeWhile(e => e <= d)
                .Union(integerList.Skip(k))
                .OrderByDescending(e => e);
        }

        public static IEnumerable<string> Task9(IEnumerable<string> stringList)
        {
            return stringList
                .Where(str => str.Length > 0)
                .GroupBy(str => str[0])
                .OrderByDescending(e => e.Sum(c => c.Length))
                .ThenBy(e => e.Key)
                .Select(e => e.Sum(c => c.Length) + "-" + e.Key);
        }

        public static IEnumerable<string> Task10(IEnumerable<string> stringList)
        {
            return stringList
                .OrderBy(str => str)
                .GroupBy(str => str.Length)
                .Select(e => e
                    .Aggregate(string.Empty, (x, y) => x + y.Substring(y.Length - 1).ToUpper())
                    )
                .OrderByDescending(e => e.Length);
        }

        #endregion

        #region Advance

        public static IEnumerable<YearSchoolStat> Task11(IEnumerable<Entrant> nameList)
        {
            return nameList
                .GroupBy(n => n.Year)
                .Select(e => new YearSchoolStat
                {
                    Year = e.Key,
                    NumberOfSchools = e.Select(x => x.SchoolNumber).Distinct().Count()
                })
                .OrderBy(e => e.NumberOfSchools)
                .ThenBy(e => e.Year);
        }

        public static IEnumerable<NumberPair> Task12(IEnumerable<int> integerList1, IEnumerable<int> integerList2)
        {
            return integerList1
                .Join(integerList2,
                    i1 => i1.ToString().Substring(i1.ToString().Length - 1),
                    i2 => i2.ToString().Substring(i2.ToString().Length - 1),
                    (i1, i2) => new NumberPair { Item1 = i1, Item2 = i2 })
                .OrderBy(e => e.Item1)
                .ThenBy(e => e.Item2);
        }

        public static IEnumerable<YearSchoolStat> Task13(IEnumerable<Entrant> nameList, IEnumerable<int> yearList)
        {
            return yearList
                .GroupJoin(nameList
                    .GroupBy(e => e.Year)
                    .Select(e => new 
                        { 
                            Year = e.Key, 
                            NumberOfSchools = e.Select(x => x.SchoolNumber).Distinct().Count() 
                        }),
                y => y,
                n => n.Year,
                (x, y) => new { Year = x, Numbers = y })
                .SelectMany(x =>
                    x.Numbers.DefaultIfEmpty(),
                    (x, y) => new YearSchoolStat
                    {
                        Year = x.Year,
                        NumberOfSchools = y == null ? 0 : y.NumberOfSchools
                    })
                .OrderBy(e => e.NumberOfSchools)
                .ThenBy(e => e.Year);
        }

        public static IEnumerable<MaxDiscountOwner> Task14(IEnumerable<Supplier> supplierList,
                IEnumerable<SupplierDiscount> supplierDiscountList)
        {
            return supplierDiscountList
                .GroupJoin(supplierList,
                    d => d.SupplierId,
                    s => s.Id,
                    (x, y) => new
                    {
                        ShopName = x.ShopName,
                        Discount = x.Discount,
                        Supp = y.FirstOrDefault()
                    })
                .GroupBy(e => e.ShopName)
                .Select(e => new MaxDiscountOwner
                {
                    ShopName = e.Key,
                    Owner = e.OrderBy(x => x.Supp.Id).FirstOrDefault(x => x.Discount == e.Max(y => y.Discount)).Supp,
                    Discount = e.Max(y => y.Discount)
                })  
                .OrderBy(e => e.ShopName);
        }

        public static IEnumerable<CountryStat> Task15(IEnumerable<Good> goodList,
            IEnumerable<StorePrice> storePriceList)
        {
            return goodList
                .GroupJoin(storePriceList,
                    g => g.Id,
                    pr => pr.GoodId,
                    (x, y) => new 
                    {
                        GoodId = x.Id,
                        Country = x.Country,
                        MinPrice = y.DefaultIfEmpty(new StorePrice()).Min(e => e.Price),
                        Stores = y.DefaultIfEmpty(new StorePrice()).AsEnumerable()
                    })
                .GroupBy(e => e.Country)
                .Select(e => new CountryStat
                {
                    Country = e.Key,
                    MinPrice = e.Min(x => x.MinPrice),
                    StoresNumber = e.SelectMany(x => x.Stores).Where(x => x.Shop != null).GroupBy(x => x.Shop).Count()
                })
                .OrderBy(e => e.Country);
        }

        #endregion

    }
}
