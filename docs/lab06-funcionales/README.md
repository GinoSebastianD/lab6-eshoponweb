# Laboratorio 06 · Pruebas funcionales

Cinco integrantes, cinco funcionalidades y 18 casos de caja negra en eShopOnWeb. En la ejecución documentada aprobaron 18 de 18 en 90.321 segundos.

[Informe PDF](Informe_Lab06_Pruebas_Funcionales.pdf) · [Reporte HTML](xunit.html) · [XML de xUnit](xunit.xml) · [Capturas](capturas/) · [GitHub Project Pruebas](https://github.com/users/GinoSebastianD/projects/2)

## Organización y seguimiento

El monorepo mantiene la aplicación en `src/`, las pruebas unitarias en sus proyectos de `tests/` y el proyecto independiente de pruebas funcionales en [functional-tests/Lab06.Selenium](../../functional-tests/Lab06.Selenium/). Este último tiene su propio .csproj y usa el navegador para comunicarse con la aplicación por HTTPS. Cada fila identifica una tarea creada en Pruebas y convertida en issue.

| Integrante | Funcionalidad | Implementación | Issue |
|---|---|---|---|
| Integrante 1 | Crear productos | [CreateProductTests](../../functional-tests/Lab06.Selenium/CreateProductTests.cs) | [#7](https://github.com/GinoSebastianD/lab6-eshoponweb/issues/7) |
| Integrante 2 | Editar productos | [EditProductTests](../../functional-tests/Lab06.Selenium/EditProductTests.cs) | [#8](https://github.com/GinoSebastianD/lab6-eshoponweb/issues/8) |
| Integrante 3 | Eliminar productos | [DeleteProductTests](../../functional-tests/Lab06.Selenium/DeleteProductTests.cs) | [#9](https://github.com/GinoSebastianD/lab6-eshoponweb/issues/9) |
| Integrante 4 | Filtrar el catálogo | [CatalogFilterTests](../../functional-tests/Lab06.Selenium/CatalogFilterTests.cs) | [#10](https://github.com/GinoSebastianD/lab6-eshoponweb/issues/10) |
| Integrante 5 | Actualizar el carrito | [BasketQuantityTests](../../functional-tests/Lab06.Selenium/BasketQuantityTests.cs) | [#11](https://github.com/GinoSebastianD/lab6-eshoponweb/issues/11) |

Commits por integrante: [1 · creación](https://github.com/GinoSebastianD/lab6-eshoponweb/commit/a3b22bd), [2 · edición](https://github.com/GinoSebastianD/lab6-eshoponweb/commit/802988b), [3 · eliminación](https://github.com/GinoSebastianD/lab6-eshoponweb/commit/0c6f6fc), [4 · filtros](https://github.com/GinoSebastianD/lab6-eshoponweb/commit/7b164d2) y [5 · carrito](https://github.com/GinoSebastianD/lab6-eshoponweb/commit/f1dfa48).

## Casos verificados

| Caso | Funcionalidad | Escenario | Valores | Resultado esperado |
|---|---|---|---|---|
| CP01 | Crear productos | Guardar un producto válido | 12.50 | Una nueva fila con los cinco datos del producto. |
| CP02 | Crear productos | Guardar en el límite inferior | 0.01 | Alta con precio 0.01. |
| CP03 | Crear productos | Guardar en el límite superior | 1000 | Alta con precio 1000. |
| CP04 | Crear productos | Enviar un precio inferior | 0 | Mensaje de intervalo. La lista conserva su tamaño. |
| CP05 | Crear productos | Superar el límite superior | 1000.01 | Mensaje de intervalo. La lista conserva su tamaño. |
| CP06 | Crear productos | Exceder la precisión | 12.345 | Mensaje de máximo dos decimales. |
| CP07 | Crear productos | Omitir el nombre | Nombre vacío | Mensaje de nombre obligatorio. |
| CP08 | Crear productos | Omitir la descripción | Descripción vacía | Mensaje de descripción obligatoria. |
| CP09 | Crear productos | Cancelar un alta válida | 12.50 | El formulario cierra y el producto no aparece. |
| ED01 | Editar productos | Guardar el nuevo precio | 15.75 | La fila del mismo producto muestra 15.75. |
| ED02 | Editar productos | Cancelar el cambio | 28.50 | El catálogo conserva el precio anterior. |
| EL01 | Eliminar productos | Confirmar la eliminación | .NET Blue Sweatshirt | Desaparece el producto y el total baja en uno. |
| EL02 | Eliminar productos | Cancelar la eliminación | Kudu Purple Sweatshirt | La fila y el total se conservan. |
| FI01 | Filtrar el catálogo | Filtrar solo por tipo | Mug | Aparecen exactamente las dos tazas del catálogo. |
| FI02 | Filtrar el catálogo | Combinar marca y tipo | .NET + Mug | Aparece solo .NET Black & White Mug. |
| FI03 | Filtrar el catálogo | Buscar sin coincidencias | Azure + Mug | Lista vacía y mensaje sin resultados. |
| CA01 | Actualizar el carrito | Cambiar cantidad | 2 × 8.50 | Cantidad 2 y total de 17.00. |
| CA02 | Actualizar el carrito | Retirar el único artículo | 0 unidades | Mensaje de carrito vacío y sin campo de cantidad. |

## Reproducir

En Windows con PowerShell 7, .NET 10 y Chrome, desde la raíz del repositorio:

```powershell
pwsh -File functional-tests/Lab06.Selenium/run-lab.ps1
```

El script compila la tienda y su API, las inicia con datos temporales en localhost (puertos 5001 y 5099) y ejecuta xUnit. Selenium abre y cierra un navegador por caso. Las suites administrativas usan el gestor de productos de demostración. Para un ChromeDriver preinstalado, indicar su carpeta en CHROMEDRIVER_DIR. Los resultados quedan en TestResults, o en la ruta indicada con -Results. En la copia pública de los reportes se abreviaron las rutas locales y se omitieron identificadores del equipo.
