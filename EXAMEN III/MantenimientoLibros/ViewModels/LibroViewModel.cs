using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MantenimientoLibros.Commands;
using MantenimientoLibros.Models;
using MantenimientoLibros.Repositories;

namespace MantenimientoLibros.ViewModels
{
    public class LibroViewModel : INotifyPropertyChanged
    {
        private readonly ILibroRepository _repository;

        public ObservableCollection<Libro> ListaLibros { get; set; }

        public ObservableCollection<Editorial> ListaEditoriales { get; set; }
        public ObservableCollection<Autor> ListaAutores { get; set; }

        private Libro _libroActual;
        public Libro LibroActual
        {
            get => _libroActual;
            set
            {
                _libroActual = value;
                OnPropertyChanged(nameof(LibroActual));
                if (_libroActual != null && !string.IsNullOrEmpty(_libroActual.TitleId))
                {
                    SincronizarAutores(_libroActual.TitleId);
                }
            }
        }

        public ICommand NuevoCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand CancelarCommand { get; }
        public ICommand EliminarCommand { get; }

        public LibroViewModel()
        {
            _repository = new LibroRepositoryImpl();

            ListaLibros = new ObservableCollection<Libro>();
            ListaEditoriales = new ObservableCollection<Editorial>();
            ListaAutores = new ObservableCollection<Autor>();

            NuevoCommand = new RelayCommand(Nuevo);
            GuardarCommand = new RelayCommand(Guardar, PuedeGuardar);
            CancelarCommand = new RelayCommand(Cancelar);
            EliminarCommand = new RelayCommand(Eliminar, PuedeEliminar);

            CargarDatosIniciales();
        }

        private void CargarDatosIniciales()
        {
            var editoriales = _repository.ObtenerEditoriales();
            foreach (var ed in editoriales) ListaEditoriales.Add(ed);

            var autores = _repository.ObtenerAutores();
            foreach (var au in autores) ListaAutores.Add(au);

            RecargarLibros();
            Nuevo(null); 
        }

        private void RecargarLibros()
        {
            ListaLibros.Clear();
            var libros = _repository.ObtenerTodos();
            foreach (var lib in libros) ListaLibros.Add(lib);
        }

        private void SincronizarAutores(string titleId)
        {
            foreach (var au in ListaAutores) au.IsSelected = false;

            var autoresIds = _repository.ObtenerAutoresPorLibro(titleId);

            foreach (var au in ListaAutores)
            {
                if (autoresIds.Contains(au.AuId))
                    au.IsSelected = true;
            }
        }

        private void Nuevo(object obj)
        {
            LibroActual = new Libro { PubDate = DateTime.Now };
            foreach (var au in ListaAutores) au.IsSelected = false;
        }

        private bool PuedeGuardar(object obj) => LibroActual != null && !string.IsNullOrWhiteSpace(LibroActual.TitleId);
        private bool PuedeEliminar(object obj) => LibroActual != null && !string.IsNullOrWhiteSpace(LibroActual.TitleId);

        private void Guardar(object obj)
        {
            try
            {
                var autoresSeleccionados = ListaAutores.Where(a => a.IsSelected).Select(a => a.AuId).ToList();

                _repository.Guardar(LibroActual, autoresSeleccionados);

                MessageBox.Show("Libro guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                RecargarLibros();
                Nuevo(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancelar(object obj) => Nuevo(null);

        private void Eliminar(object obj)
        {
            if (MessageBox.Show("¿Desea eliminar este libro y sus relaciones?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    _repository.Eliminar(LibroActual.TitleId);
                    RecargarLibros();
                    Nuevo(null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}