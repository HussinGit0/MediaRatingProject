using MediaRatingProject.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingProject.API.Requests
{
    public class FailedRequest: IRequest
    {
        public void Accept(IRequestVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
