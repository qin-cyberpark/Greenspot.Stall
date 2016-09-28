using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models.ViewModels
{
    public class ErrorViewModel
    {
        public enum ErrorType { Customer, General, FileNotFound, UnauthorizedAccess, PayFailed }
        public ErrorType Type { get; set; }
        public string ImageFile { get; set; }
        public string BackUrl { get; set; }
        public string Message { get; set; }
        public ErrorViewModel()
        {
            Type = ErrorType.General;
        }
        public ErrorViewModel(ErrorType type)
        {
            Type = type;
        }
    }
}