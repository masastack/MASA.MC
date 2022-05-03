// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Data.App.ECommerce.Dto
{
    public class RelatedGoodsDto
    {
        public string ImgUrl { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string Brand { get; set; } = default!;

        public int Rating { get; set; }

        public decimal Price { get; set; }

        public RelatedGoodsDto(string name, string brand, string imgUrl, decimal price, int rating)
        {
            Name = name;
            Brand = brand;
            ImgUrl = imgUrl;
            Price = price;
            Rating = rating;
        }
    }
}