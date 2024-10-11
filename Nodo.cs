public class Nodo
{
    public string Valor { get; set; }          // Valor del nodo
    public List<Nodo> Hijos { get; set; }      // Lista de nodos hijos

    public Nodo(string valor)
    {
        Valor = valor;                         // Inicializa el valor del nodo
        Hijos = new List<Nodo>();              // Inicializa la lista de hijos
    }

    public void AgregarHijo(Nodo hijo)
    {
        Hijos.Add(hijo);                       // Agrega un hijo a la lista
    }

    public void Imprimir(string prefijo = "", bool esUltimo = true) // Método para imprimir el árbol
    {
        Console.WriteLine(prefijo + (esUltimo ? "└── " : "├── ") + Valor); // Imprime el valor con prefijo

        prefijo += esUltimo ? "    " : "│   "; // Ajusta el prefijo para los hijos

        for (int i = 0; i < Hijos.Count; i++)
        {
            Hijos[i].Imprimir(prefijo, i == Hijos.Count - 1); // Llama recursivamente a imprimir los hijos
        }
    }
}
