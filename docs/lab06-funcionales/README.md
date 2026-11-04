# Laboratorio 06 · Pruebas funcionales

El objeto de prueba es **Crear producto** en la administración de eShopOnWeb. Selenium WebDriver maneja Chrome y xUnit v3 ejecuta un método por caso. Cada método inicia sesión en una ventana nueva con el gestor de productos de demostración y cierra el navegador al terminar. Se mantienen la marca y el tipo que ofrece inicialmente el formulario. Cada alta usa un nombre único.

## Casos de caja negra

| Caso | Funcionalidad | Escenario o acción | Valores de prueba | Resultado esperado |
|---|---|---|---|---|
| CP01 | Crear producto | Guardar un producto válido | Nombre único, descripción, precio 12.50 | Aparece una fila con el nombre, descripción, marca, tipo y precio elegidos. |
| CP02 | Crear producto | Guardar el mínimo permitido | 0.01 | Se crea el producto con ese precio. |
| CP03 | Crear producto | Guardar el máximo permitido | 1000 | Se crea el producto con ese precio. |
| CP04 | Crear producto | Enviar un precio bajo el mínimo | 0 | Se indica el intervalo permitido y la lista conserva su tamaño. |
| CP05 | Crear producto | Superar el máximo por un centavo | 1000.01 | Se indica el intervalo permitido y la lista conserva su tamaño. |
| CP06 | Crear producto | Escribir tres decimales | 12.345 | Se indica el máximo de dos decimales y la lista conserva su tamaño. |
| CP07 | Crear producto | Omitir el nombre | Nombre vacío, precio 12.50 | Aparece el mensaje de nombre obligatorio y la lista conserva su tamaño. |
| CP08 | Crear producto | Omitir la descripción | Descripción vacía, precio 12.50 | Aparece el mensaje de descripción obligatoria y la lista conserva su tamaño. |
| CP09 | Crear producto | Cancelar con datos válidos | Nombre único, descripción, 12.50 | Se cierra el formulario y el producto no aparece en el catálogo. |

Se utilizaron clases de equivalencia para los campos de texto y la precisión del precio, y valores límite para el intervalo de 0.01 a 1000, ambos inclusive. Los demás campos son válidos en cada caso.

## Reproducción

Requiere Windows, PowerShell 7, SDK .NET 10 y Google Chrome. Desde la raíz del repositorio, ejecutar en una consola nueva:

```powershell
pwsh -File tests/Lab06.Selenium/run-lab.ps1
```

El script compila la web y la API, las inicia en los puertos locales 5001 y 5099 con datos en memoria y ejecuta la suite. El certificado temporal solo se usa durante la ejecución local. Selenium Manager puede descargar el controlador automáticamente. Para utilizar uno instalado, configurar `CHROMEDRIVER_DIR` con la carpeta de ChromeDriver de la misma compilación que Chrome.

El resultado por defecto se guarda en `tests/Lab06.Selenium/TestResults`. Se puede indicar otra carpeta con `-Results`. El HTML y el XML proceden del ejecutor nativo de xUnit. Las capturas del formulario se obtienen con Selenium.

## Entrega

[Informe breve en PDF](Informe_Lab06_Pruebas_Funcionales.pdf) · [Resultados xUnit en HTML](xunit.html) · [Resultados en XML](xunit.xml) · [Capturas](capturas/)

En la ejecución documentada aprobaron los nueve casos en 54.231 segundos. Para publicar las evidencias se abreviaron las rutas locales y se omitieron los identificadores del equipo y del usuario.
