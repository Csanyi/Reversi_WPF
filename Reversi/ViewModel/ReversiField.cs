using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.ViewModel
{
    class ReversiField : ViewModelBase
    {
        private bool isWhite;
        public bool IsWhite
        {
            get { return isWhite; }
            set
            {
                if(isWhite != value)
                {
                    isWhite = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isBlack;
        public bool IsBlack
        {
            get { return isBlack; }
            set
            {
                if (isBlack != value)
                {
                    isBlack = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isEmpty;
        public bool IsEmpty
        {
            get { return isEmpty; }
            set
            {
                if (isEmpty != value)
                {
                    isEmpty = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    OnPropertyChanged();
                }
            }
        }

        public int X { get; set; }

        public int Y { get; set; }

        public int Number { get; set; }

        public DelegateCommand StepCommand { get; set; }
    }
}
