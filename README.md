#UHost
Germán Valencia (gvalenc4 at eafit.edu.co)

##Descripción General
Una aplicación para alojar archivos genéricos (con enfoque a imágenes) con título y descripción

##1. Análisis
###1.1 Requisitos funcionales
* Iniciar sesión con proveedores sociales
* Listar los archivos ya existentes
* Subir archivos nuevos
###1.2 Requisitos no funcionales
* Autenticación externa a cargo de [Microsoft Azure ADB2C][1]
* Framework Web [AspNetCore][2] 1.1
* Servidor Web [Kestrel][3] (Desarrollo, Testing, Staging) y Microsoft IIS (Producción)
* Base de datos [LiteDB][4] (Desarrollo, Testing) y [Microsoft CosmosDB][5] (Staging, Producción)
* Almacenamiento de archivos en el disco local (Desarrollo, Testing) y [Azure BlobStorage][6] (Staging, Producción)
* Ejecución sobre Linux, Windows y OSX (Desarrollo, Testing), Docker (Staging) y Windows Server (Producción)
##2. Desarrollo
Se utilizó Visual Studio Community 2017 para generar la plantilla inicial del proyecto, la cual trae unicamente una pantalla inicial maquetada con Twitter Bootstrap y código para autenticación local. Todo este código fue eliminado para escribir vistas desde cero, reciclando únicamente la preconfiguración de Twitter Bootstrap.
##3. Diseño
###3.1 Modelo de datos principal
``` c#
public class HostedFile {
    public Guid Id { get; set; }
    public string OwnerId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string FileSlug { get; set; }
    public string ContentType { get; set; }
    public DateTime CreationDate { get; set; }
}
``` 
###3.2 Servicios Web
La aplicación todavía no cuenta con servicios Web consumibles directamente

##4. Despliegue en el Servidor CentOS 7.x DCA EAFIT
###Instalar Docker
```
sudo yum install docker -y
```
###Arrancar y habilitar el servicio Docker
```
sudo systemctl start docker
sudo systemctl enable docker
```
###Instalar el SDK dotnet
```
wget https://download.microsoft.com/download/F/A/A/FAAE9280-F410-458E-8819-279C5A68EDCF/dotnet-sdk-2.0.0-preview2-006497-linux-x64.tar.gz
sudo mkdir -p /opt/dotnet
sudo tar zxf dotnet-sdk-2.0.0-preview2-006497-linux-x64.tar.gz -C /opt/dotnet
sudo ln -s /opt/dotnet/dotnet /usr/local/bin
```
###Clonar y publicar el proyecto
```
git clone https://github.com/Machinarius/UHost.git UHost
cd UHost/UHost
dotnet restore
dotnet publish -c Release -o publish
```
###Crear y arrancar la imagen Docker
```
sudo docker build -t uhost .
sudo docker -d -p 5000:5000 --restart unless-stopped uhost
```
La bandera ```-d``` convierte la app en un daemon Docker, llevando su I/O de consola al fondo.
La opción  ```-p``` automáticamente configura el puerto interno 5000 del contenedor al puerto externo 5000 del servidor.
La opción ```--restart unless-stopped``` configura Docker para iniciar el contenedor cada vez que se inicie el servicio de Docker, lo que lo convierte en un servicio del sistema que tolera reinicios de la máquina o actualizaciones de Docker. 

###Verificar el inicio de la app
Es necesario confiar en el certificado SSL de firma propia que se usa en el entorno Staging por requerimiento de Azure ADB2C
```
firefox https://10.131.137.183:5000/
```

[1]: https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-overview
[2]: https://docs.microsoft.com/en-us/aspnet/core/
[3]: https://github.com/aspnet/KestrelHttpServer
[4]: http://www.litedb.org/
[5]: https://docs.microsoft.com/en-us/azure/cosmos-db/introduction
[6]: https://azure.microsoft.com/en-us/services/storage/blobs/