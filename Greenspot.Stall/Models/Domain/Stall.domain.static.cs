﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Greenspot.Stall.Models
{
    public partial class Stall
    {
        public static IList<Stall> FindByUserId(string userId, StallEntities db)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            return db.Stalls.Where(x => x.UserId.Equals(userId)).ToList();
        }

        public static IList<Stall> FindByVendPrefix(string prefix, StallEntities db)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                return null;
            }

            return db.Stalls.Where(x => x.Prefix.Equals(prefix)).ToList();
        }

        public static Stall FindByName(string name, StallEntities db)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            return db.Stalls.FirstOrDefault(x => x.StallName.Equals(name));
        }

        public static Stall FindById(int id, StallEntities db)
        {

            return db.Stalls.Include(x => x.Products).FirstOrDefault(x => x.Id == id);
        }

        public static Stall FindByRetailerId(string id, StallEntities db)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            return db.Stalls.Include(x => x.Products).FirstOrDefault(x => x.RetailerId.Equals(id));
        }

        public static Stall FindByRetailerIdAndSuppilerName(string retailerId, string supplierName, StallEntities db)
        {
            if (string.IsNullOrEmpty(retailerId))
            {
                return null;
            }

            var stalls = db.Stalls.Include(x => x.Products).Where(x => retailerId.Equals(x.RetailerId));
            var isUnion = stalls.Any(x => x.IsUnion);
            if (!isUnion)
            {
                //normal stall
                return stalls.FirstOrDefault();
            }
            else
            {
                //is union
                var baseStall = stalls.FirstOrDefault(x => StallStatus.UnionMain.Equals(x.Status) && x.IsUnion);
                if (string.IsNullOrEmpty(supplierName))
                {
                    //not supplier infor
                    return baseStall;
                }

                //got stall
                var stall = stalls.FirstOrDefault(x => supplierName.Equals(x.StallName) && x.IsUnion);
                return stall ?? baseStall;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="category"></param>
        /// <param name="area"></param>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IList<Stall> Search(StallEntities db, string category, string area, string keyword, int page = 0, int pageSize = 10)
        {
            var result = db.Stalls.Include(x => x.Products).Where(x => StallStatus.Online.Equals(x.Status));
            if (!string.IsNullOrEmpty(area))
            {
                result = result.Where(x => x.Area.StartsWith(area));
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                result = result.Where(x => x.StallName.ToLower().Contains(keyword.ToLower()));
            }

            if (!string.IsNullOrEmpty(category))
            {
                result = result.Where(x => x.StallType.Equals(category));
            }

            return result.OrderBy(x => x.StallName).Skip(pageSize * page).Take(pageSize).ToList();
        }

        /// <summary>
        /// get recommend stall
        /// </summary>
        /// <param name="category"></param>
        /// <param name="area"></param>
        /// <param name="db"></param>
        /// <param name="takeAmount"></param>
        /// <returns></returns>
        public static IList<Stall> GetRecommend(string category, string area, StallEntities db, int takeAmount = 50)
        {
            var result = db.Stalls.Include(x => x.Products).Where(x => StallStatus.Online.Equals(x.Status));
            if (!string.IsNullOrEmpty(area))
            {
                result = result.Where(x => x.Area.StartsWith(area));
            }

            if (!string.IsNullOrEmpty(category))
            {
                result = result.Where(x => x.StallType.Equals(category));
            }

            return result.OrderBy(x => x.RecommendIndex).Take(takeAmount).ToList();
        }

        public static OperationResult<Stall> CraeteStall(string userId, string name, string prefix, StallEntities db)
        {
            var result = new OperationResult<Stall>(false);
            if (string.IsNullOrEmpty(userId))
            {
                result.Message = string.Format("UserId不能为空", prefix);
            }
            else if (string.IsNullOrEmpty(name))
            {
                result.Message = string.Format("铺名不能为空", prefix);
            }
            //else if (string.IsNullOrEmpty(prefix))
            //{
            //    result.Message = "Vend前缀不能为空";
            //}
            else if (Stall.FindByName(name, db) != null)
            {
                result.Message = string.Format("铺名 {0} 已占用", name);
            }
            //else if (Stall.FindByVendPrefix(prefix, db) != null)
            //{
            //    result.Message = string.Format("{0}.vendhq.com 已注册", prefix);
            //}
            else
            {
                var stall = new Stall() { UserId = userId, StallName = name, Prefix = prefix };
                result.Data = db.Stalls.Add(stall);
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    result.Message = string.Format("无法保存店铺 {0}", name);
                }
            }

            if (result.Data != null)
            {
                result.Succeeded = true;
            }

            return result;
        }
    }
}