using System;
using System.Collections.Generic;

namespace HoloCrew.Models
{
    // Categoría de productos con estructura jerárquica (categoría principal puede tener subcategorías).
    // Slug es una versión amigable para URL (ej: "tshirts" en vez de "Camisetas").
    // IsMainCategory true si no tiene categoría padre.
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; } // para la url: "tshirts", "hoodies", "cargo"
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public string ImageUrl { get; set; }

        // jerarquía: una categoría puede tener padre y varias hijas
        public int? ParentCategoryId { get; set; }
        public Category ParentCategory { get; set; }
        public List<Category> SubCategories { get; set; } = new List<Category>();

        public int ProductCount { get; set; } // cuántos productos tiene esta categoría

        public bool IsMainCategory => ParentCategoryId == null;
        public bool HasSubCategories => SubCategories != null && SubCategories.Count > 0;
    }

    // Categorías fijas de HoloCrew basadas en la tienda real.
    // Aquí están definidas todas: Shop All, Tops, Bottoms, Footwear, Accessories,
    // y sus respectivas subcategorías (T-Shirts, Hoodies, Cargo Pants, etc.).
    public static class HoloCrewCategories
    {
        // ========== CATEGORÍAS PRINCIPALES ==========
        public static readonly Category ShopAll = new Category { Id = 1, Name = "Shop All", Slug = "all" };
        public static readonly Category Tops = new Category { Id = 2, Name = "Tops", Slug = "tops" };
        public static readonly Category Bottoms = new Category { Id = 3, Name = "Bottoms", Slug = "bottoms" };
        public static readonly Category Footwear = new Category { Id = 4, Name = "Footwear", Slug = "footwear" };
        public static readonly Category Accessories = new Category { Id = 5, Name = "Accessories", Slug = "accessories" };

        // ========== SUBCATEGORÍAS DE SHOP ALL ==========
        public static readonly Category BlackWeek = new Category { Id = 10, Name = "Black Week", Slug = "blackweek", ParentCategoryId = 1 };
        public static readonly Category NewArrivals = new Category { Id = 11, Name = "New Arrivals", Slug = "new", ParentCategoryId = 1 };
        public static readonly Category SoftsCollection = new Category { Id = 12, Name = "Softs Collection", Slug = "softs", ParentCategoryId = 1 };
        public static readonly Category ClassicCollection = new Category { Id = 13, Name = "Classic Collection", Slug = "classic", ParentCategoryId = 1 };
        public static readonly Category Activewear = new Category { Id = 14, Name = "Activewear", Slug = "activewear", ParentCategoryId = 1 };
        public static readonly Category Tracksuits = new Category { Id = 15, Name = "Tracksuits", Slug = "tracksuits", ParentCategoryId = 1 };

        // ========== SUBCATEGORÍAS DE TOPS ==========
        public static readonly Category TShirts = new Category { Id = 20, Name = "T-Shirts", Slug = "tshirts", ParentCategoryId = 2 };
        public static readonly Category Hoodies = new Category { Id = 21, Name = "Hoodies", Slug = "hoodies", ParentCategoryId = 2 };
        public static readonly Category TrackJackets = new Category { Id = 22, Name = "Track Jackets", Slug = "trackjackets", ParentCategoryId = 2 };
        public static readonly Category Jerseys = new Category { Id = 23, Name = "Jerseys", Slug = "jerseys", ParentCategoryId = 2 };
        public static readonly Category Knitwear = new Category { Id = 24, Name = "Knitwear", Slug = "knitwear", ParentCategoryId = 2 };
        public static readonly Category Jackets = new Category { Id = 25, Name = "Jackets", Slug = "jackets", ParentCategoryId = 2 };

        // ========== SUBCATEGORÍAS DE BOTTOMS ==========
        public static readonly Category DenimPants = new Category { Id = 30, Name = "Denim Pants", Slug = "denim", ParentCategoryId = 3 };
        public static readonly Category CargoPants = new Category { Id = 31, Name = "Cargo Pants", Slug = "cargo", ParentCategoryId = 3 };
        public static readonly Category Joggers = new Category { Id = 32, Name = "Joggers", Slug = "joggers", ParentCategoryId = 3 };
        public static readonly Category TrackPants = new Category { Id = 33, Name = "Track Pants", Slug = "trackpants", ParentCategoryId = 3 };
        public static readonly Category Jorts = new Category { Id = 34, Name = "Jorts", Slug = "jorts", ParentCategoryId = 3 };
        public static readonly Category Shorts = new Category { Id = 35, Name = "Shorts", Slug = "shorts", ParentCategoryId = 3 };
        public static readonly Category Swimshorts = new Category { Id = 36, Name = "Swimshorts", Slug = "swimshorts", ParentCategoryId = 3 };
        public static readonly Category Underwear = new Category { Id = 37, Name = "Underwear", Slug = "underwear", ParentCategoryId = 3 };

        // ========== SUBCATEGORÍAS DE FOOTWEAR ==========
        public static readonly Category ArmboLows = new Category { Id = 40, Name = "Armbo Lows", Slug = "armbo", ParentCategoryId = 4 };
        public static readonly Category Vortex = new Category { Id = 41, Name = "Vortex", Slug = "vortex", ParentCategoryId = 4 };
        public static readonly Category Venture = new Category { Id = 42, Name = "Venture", Slug = "venture", ParentCategoryId = 4 };
        public static readonly Category Vitoria = new Category { Id = 43, Name = "Vitoria", Slug = "vitoria", ParentCategoryId = 4 };
        public static readonly Category VSlides = new Category { Id = 44, Name = "V-Slides", Slug = "vslides", ParentCategoryId = 4 };

        // ========== SUBCATEGORÍAS DE ACCESSORIES ==========
        public static readonly Category Caps = new Category { Id = 50, Name = "Caps", Slug = "caps", ParentCategoryId = 5 };
        public static readonly Category Bags = new Category { Id = 51, Name = "Bags", Slug = "bags", ParentCategoryId = 5 };
        public static readonly Category Beanies = new Category { Id = 52, Name = "Beanies", Slug = "beanies", ParentCategoryId = 5 };
        public static readonly Category Cardholder = new Category { Id = 53, Name = "Cardholder", Slug = "cardholder", ParentCategoryId = 5 };
        public static readonly Category Belts = new Category { Id = 54, Name = "Belts", Slug = "belts", ParentCategoryId = 5 };
        public static readonly Category Rings = new Category { Id = 55, Name = "Rings", Slug = "rings", ParentCategoryId = 5 };
        public static readonly Category Rugs = new Category { Id = 56, Name = "Rugs", Slug = "rugs", ParentCategoryId = 5 };

        // devuelve todas las categorías principales con sus subcategorías ya metidas dentro
        public static List<Category> GetAllCategories()
        {
            return new List<Category>
            {
                new Category
                {
                    Id = 1, Name = "Shop All", Slug = "all",
                    SubCategories = new List<Category>
                    {
                        BlackWeek, NewArrivals, SoftsCollection, ClassicCollection, Activewear, Tracksuits
                    }
                },

                new Category
                {
                    Id = 2, Name = "Tops", Slug = "tops",
                    SubCategories = new List<Category>
                    {
                        TShirts, Hoodies, TrackJackets, Jerseys, Knitwear, Jackets
                    }
                },

                new Category
                {
                    Id = 3, Name = "Bottoms", Slug = "bottoms",
                    SubCategories = new List<Category>
                    {
                        DenimPants, CargoPants, Joggers, TrackPants, Jorts, Shorts, Swimshorts, Underwear
                    }
                },

                new Category
                {
                    Id = 4, Name = "Footwear", Slug = "footwear",
                    SubCategories = new List<Category>
                    {
                        ArmboLows, Vortex, Venture, Vitoria, VSlides
                    }
                },

                new Category
                {
                    Id = 5, Name = "Accessories", Slug = "accessories",
                    SubCategories = new List<Category>
                    {
                        Caps, Bags, Beanies, Cardholder, Belts, Rings, Rugs
                    }
                }
            };
        }

        // busca una categoría (principal o subcategoría) por su slug
        public static Category GetBySlug(string slug)
        {
            if (string.IsNullOrEmpty(slug)) return null;

            slug = slug.ToLower();

            var allCategories = GetAllCategories();

            foreach (var category in allCategories)
            {
                if (category.Slug == slug) return category;

                if (category.SubCategories != null)
                {
                    var subCat = category.SubCategories.FirstOrDefault(s => s.Slug == slug);
                    if (subCat != null) return subCat;
                }
            }

            return null;
        }

        // saca el id de una subcategoría a partir de su slug
        public static int? GetSubCategoryIdBySlug(string slug)
        {
            var category = GetBySlug(slug);
            return category?.Id;
        }
    }
}