using GestionOrdenesMVVM.Commands;
using GestionOrdenesMVVM.Models;
using GestionOrdenesMVVM.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace GestionOrdenesMVVM.ViewModels
{
    public class OrdenViewModel : INotifyPropertyChanged
    {
        private readonly IOrdenRepository _repository;

        // Colecciones para la vista
        public ObservableCollection<Orden> ListaOrdenes { get; set; }
        public List<string> ListaEmpleados { get; set; }
        public List<string> ListaCiudades { get; set; }

        // Propiedades del formulario
        private string _orderId;
        public string OrderId { get { return _orderId; } set { _orderId = value; OnPropertyChanged(); } }

        private string _cliente;
        public string Cliente { get { return _cliente; } set { _cliente = value; OnPropertyChanged(); } }

        private string _empleado;
        public string Empleado { get { return _empleado; } set { _empleado = value; OnPropertyChanged(); } }

        private string _destino;
        public string Destino { get { return _destino; } set { _destino = value; OnPropertyChanged(); } }

        private string _ciudad;
        public string Ciudad { get { return _ciudad; } set { _ciudad = value; OnPropertyChanged(); } }

        private string _monto;
        public string Monto { get { return _monto; } set { _monto = value; OnPropertyChanged(); } }

        // RadioButtons
        private bool _isSpeedy;
        public bool IsSpeedy { get { return _isSpeedy; } set { _isSpeedy = value; OnPropertyChanged(); } }

        private bool _isUnited;
        public bool IsUnited { get { return _isUnited; } set { _isUnited = value; OnPropertyChanged(); } }

        private bool _isFederal;
        public bool IsFederal { get { return _isFederal; } set { _isFederal = value; OnPropertyChanged(); } }

        // Fila seleccionada en el DataGrid
        private Orden _ordenSeleccionada;
        public Orden OrdenSeleccionada
        {
            get { return _ordenSeleccionada; }
            set
            {
                _ordenSeleccionada = value;
                OnPropertyChanged();
                if (_ordenSeleccionada != null) CargarDatosAlFormulario();
            }
        }

        // Comandos (Botones)
        public ICommand GuardarCommand { get; }
        public ICommand ActualizarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand LimpiarCommand { get; }

        public OrdenViewModel()
        {
            _repository = new OrdenRepositoryImpl();

            // Inicializar Comandos
            GuardarCommand = new RelayCommand(Guardar);
            ActualizarCommand = new RelayCommand(Actualizar);
            EliminarCommand = new RelayCommand(Eliminar);
            LimpiarCommand = new RelayCommand(Limpiar);

            // Cargar Datos Iniciales
            CargarListas();
        }

        private void CargarListas()
        {
            try
            {
                ListaEmpleados = _repository.ListarEmpleados();
                ListaCiudades = _repository.ListarCiudades();
                ListaOrdenes = new ObservableCollection<Orden>(_repository.ListarTodos());

                OnPropertyChanged(nameof(ListaEmpleados));
                OnPropertyChanged(nameof(ListaCiudades));
                OnPropertyChanged(nameof(ListaOrdenes));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la base de datos: {ex.Message}");
            }
        }

        private void CargarDatosAlFormulario()
        {
            OrderId = OrdenSeleccionada.OrderID.ToString();
            Cliente = OrdenSeleccionada.Cliente;
            Empleado = OrdenSeleccionada.Empleado;
            Ciudad = OrdenSeleccionada.CiudadDestino;
            Destino = "Destino Genérico";
            Monto = OrdenSeleccionada.Monto.ToString();

            IsSpeedy = OrdenSeleccionada.Transportista == "Speedy Express";
            IsUnited = OrdenSeleccionada.Transportista == "United Package";
            IsFederal = OrdenSeleccionada.Transportista == "Federal Shipping";
        }

        private void Guardar(object obj)
        {
            try
            {
                Orden nuevaOrden = new Orden
                {
                    Cliente = this.Cliente,
                    Empleado = this.Destino,
                    CiudadDestino = this.Ciudad,
                    Monto = decimal.TryParse(this.Monto, out decimal m) ? m : 0,
                    Transportista = IsSpeedy ? "Speedy Express" : IsUnited ? "United Package" : "Federal Shipping"
                };

                _repository.Insertar(nuevaOrden);
                MessageBox.Show("Orden registrada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                CargarListas();
                Limpiar(null);
            }
            catch (Exception ex) { MessageBox.Show($"Error al guardar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void Actualizar(object obj)
        {
            if (string.IsNullOrEmpty(OrderId)) return;
            try
            {
                Orden ordenAct = new Orden
                {
                    OrderID = int.Parse(this.OrderId),
                    Cliente = this.Cliente,
                    Empleado = this.Destino,
                    CiudadDestino = this.Ciudad,
                    Monto = decimal.TryParse(this.Monto, out decimal m) ? m : 0,
                    Transportista = IsSpeedy ? "Speedy Express" : IsUnited ? "United Package" : "Federal Shipping"
                };
                _repository.Actualizar(ordenAct);
                MessageBox.Show("Orden actualizada.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                CargarListas();
                Limpiar(null);
            }
            catch (Exception ex) { MessageBox.Show($"Error al actualizar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void Eliminar(object obj)
        {
            if (string.IsNullOrEmpty(OrderId)) return;
            if (MessageBox.Show("¿Eliminar esta orden?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    _repository.Eliminar(int.Parse(OrderId));
                    MessageBox.Show("Orden eliminada.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    CargarListas();
                    Limpiar(null);
                }
                catch (Exception ex) { MessageBox.Show($"Error al eliminar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }

        private void Limpiar(object obj)
        {
            OrderId = string.Empty; Cliente = string.Empty; Empleado = null;
            Destino = string.Empty; Ciudad = null; Monto = string.Empty;
            IsSpeedy = false; IsUnited = false; IsFederal = false;
            OrdenSeleccionada = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}