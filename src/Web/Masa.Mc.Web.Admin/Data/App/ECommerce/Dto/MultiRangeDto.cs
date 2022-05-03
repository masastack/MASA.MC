// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Data.App.ECommerce.Dto
{
    public class MultiRangeDto
    {
        public MultiRangeDto(RangeType rangeType, string text, double leftNumber, double rightNumber = 0)
        {
            RangeType = rangeType;
            Text = text;
            LeftNumber = leftNumber;
            RightNumber = rightNumber;
        }

        public RangeType RangeType { get; set; }

        public string Text { get; set; }

        public double LeftNumber { get; set; }

        public double RightNumber { get; set; }
    }

    public enum RangeType
    {
        All,
        Less,
        LessEqual,
        More,
        MoreEqual,
        Range
    }
}