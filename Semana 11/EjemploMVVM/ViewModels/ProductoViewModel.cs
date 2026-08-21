using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using EjemploMVVM.Commands;
using EjemploMVVM.Models;
using EjemploMVVM.Repositories;

namespace EjemploMVVM.ViewModels
{
    public class ProductoViewModel
    {
        public ObservableCollection<Producto> productos { get; set; } = new ObservableCollection<Producto>();

        public ObservableCollection<string> categorias { get; set; } = new ObservableCollection<string>();
        public string categoriaSeleccionada { get; set; } = string.Empty;

        public RelayCommand ComandoCargarProductos { get; set; }

        public string textoBuscar { get; set; } = string.Empty;

        private IProductoRepository _repository;

        public ProductoViewModel()
        {
            _repository = new ProductoRepositoryImpl();
            ComandoCargarProductos = new RelayCommand(BuscarProductos);

            CargarCategorias();
            CargarProductos();
        }

        private void CargarCategorias()
        {
            categorias.Clear();
            categorias.Add("Todas"); 
            categorias.Add("Beverages");
            categorias.Add("Condiments");
            categorias.Add("Confections");
            categorias.Add("Dairy Products");
            categorias.Add("Grains/Cereals");
            categorias.Add("Meat/Poultry");
            categorias.Add("Produce");
            categorias.Add("Seafood");

            categoriaSeleccionada = "Todas";
        }

        private void BuscarProductos()
        {
            List<Producto> lista = _repository.BuscarPorNombreYCategoria(textoBuscar, categoriaSeleccionada);

            productos.Clear();
            foreach (Producto producto in lista)
            {
                productos.Add(producto);
            }
        }

        public void CargarProductos()
        {
            List<Producto> lista = _repository.ListarTodos();
            productos.Clear();
            foreach (Producto producto in lista)
            {
                productos.Add(producto);
            }
            int cantidad = productos.Count;
        }
    }
}