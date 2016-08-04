using System;
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

        public static Stall FindByVendPrefix(string prefix, StallEntities db)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                return null;
            }

            return db.Stalls.FirstOrDefault(x => x.Prefix.Equals(prefix));
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
         
            return db.Stalls.Include(x=>x.Products).FirstOrDefault(x => x.Id == id);
        }

        public static Stall FindByRetailerId(string id, StallEntities db)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            return db.Stalls.Include(x => x.Products).FirstOrDefault(x => x.RetailerId.Equals(id));
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
            else if (string.IsNullOrEmpty(prefix))
            {
                result.Message = string.Format("Vend前缀不能为空", prefix);
            }else if (Stall.FindByName(name, db) != null)
            {
                result.Message = string.Format("铺名 {0} 已占用", name);
            }else if (Stall.FindByVendPrefix(prefix, db) != null)
            {
                result.Message = string.Format("{0}.vendhq.com 已注册", prefix);
            }else
            {
                var stall = new Stall() { UserId = userId, StallName = name, Prefix = prefix };
                result.Data = db.Stalls.Add(stall);
                db.SaveChanges();
            }

            if(result.Data == null)
            {
                result.Message = "无法保存商铺信息";
            }

            result.Succeeded = true;
            return result;
        }
    }
}