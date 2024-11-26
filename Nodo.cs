public class Nodo
{
    public string Valor { get; set; }    // Valor del nodo (como 'x', '1', '+', etc.)
    public string Tipo { get; set; }     // Tipo del nodo (puede ser "Operador", "Expresión", "Sentencia", etc.)
    public List<Nodo> Hijos { get; set; } // Lista de nodos hijos

    public Nodo(string valor, string tipo = "")
    {
        Valor = valor;
        Tipo = tipo; // El tipo puede ser 'Operador', 'Sentencia', 'Expresión', etc.
        Hijos = new List<Nodo>();
    }

    public void AgregarHijo(Nodo hijo)
    {
        Hijos.Add(hijo);
    }

    // Imprimir el árbol para depuración (opcional)
    public void Imprimir(string prefijo = "", bool esUltimo = true)
    {
        Console.WriteLine(prefijo + (esUltimo ? "└── " : "├── ") + Valor); // Imprime el valor con prefijo

        prefijo += esUltimo ? "    " : "│   "; // Ajusta el prefijo para los hijos

        for (int i = 0; i < Hijos.Count; i++)
        {
            Hijos[i].Imprimir(prefijo, i == Hijos.Count - 1); // Llama recursivamente a imprimir los hijos
        }
    }
}
