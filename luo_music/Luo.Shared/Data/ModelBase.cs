using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Luo.Shared
{
    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged<T>(Expression<Func<T>> exp)
        {
            var memberExpr = exp.Body as MemberExpression;
            if (memberExpr == null)
                throw new ArgumentException("propertyExpression should represent access to a member");
            string memberName = memberExpr.Member.Name;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
    }
}
