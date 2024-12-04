﻿using UniqloTasks.ViewModels.Brands;
using UniqloTasks.ViewModels.Products;

namespace UniqloTasks.ViewModels.Shops
{
	public class ShopVM
	{
		public IEnumerable<BrandAndProductVM> Brands { get; set; }
		public IEnumerable<ProductListItemVM> Products { get; set; }
		public int ProductCount { get; set; }

	}
}
