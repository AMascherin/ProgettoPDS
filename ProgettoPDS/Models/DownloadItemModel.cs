using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgettoPDS.Models
{
    class DownloadItemModel
    {
        private String _originalFileName;
        private String _Format;
        private long _dimension;
        private String _targetFileName;

        public string OriginalFileName
        {
            get
            {
                return _originalFileName;
            }

            set
            {
                _originalFileName = value;
            }
        }

        public string Format
        {
            get
            {
                return _Format;
            }

            set
            {
                _Format = value;
            }
        }

        public long Dimension
        {
            get
            {
                return _dimension;
            }

            set
            {
                _dimension = value;
            }
        }

        public string TargetFileName
        {
            get
            {
                return _targetFileName;
            }

            set
            {
                _targetFileName = value;
            }
        }
    }
}
