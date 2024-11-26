using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthOrDrink.Model
{
    public class Answer
    {
		private int _id;
		private int _questionId;
		private string _response;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public int QuestionId
		{
			get { return _questionId; }
			set { _questionId = value; }
		}

		public string Response
		{
			get { return _response; }
			set { _response = value; }
		}
	}
}
