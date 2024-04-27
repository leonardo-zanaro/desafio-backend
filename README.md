# API - Aluguel de Motos e Entregas
Esta api foi projetada para gerenciar operações relacionadas ao aluguel, entrega e devolução de motos. Ela oferece endpoints para criar e gerenciar usuários, cadastrar e consultar motocicletas, gerenciar entregadores, criar e gerenciar pedidos de entrega, além de fornecer funcionalidades de autenticação e segurança.
## Tecnologias utilizadas
  
  - C#
  - PostgreSQL
  - RabbitMQ
  - Docker

## Pré configuração
  - Para que a aplicação rode sem nenhum problema é necessário ter ao menos o [Docker](https://www.docker.com/) instalado na máquina.
  - Para rodar localmente será necessário mais ferramentas segue a lista
    
    - [.NET SDK 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
    - Banco de dados [PostgreSQL](https://www.postgresql.org/download/)
    - [RabbitMQ](https://www.rabbitmq.com/docs/download)
## Instalação
  1. Clone este projeto com o seguinte comando:
  
     ```git clone https://github.com/leonardo-zanaro/desafio-backend.git```
  
  2. Acesse a pasta do projeto através do seguinte comando no terminal:

     ```cd desafio-backend/src```
     
  4. No terminal execute o seguinte comando:
    
     ```docker-compose up -d```
  
  5. Com isso realizado basta acessar as URLs correspondentes nas portas configuradas.

## Funcionalidades
## Auth
- ### Login

    `POST` `/user/login`
    
    Permite que um usuário faça login.
    ```json
    {
      "username": "usuário",
      "password": "senha"
    }
    ```
    
    
- ### Criar Usuário
    `POST` `/user/create`
    
    Cria um novo usuário, caso seja admin necessário colocar a flag `isAdmin` como `true`.
    ```json
    {
      "username": "usuário",
      "password": "senha",
      "email": "e-mail",
      "isAdmin": true
    }
    ```

## Deliverer
- ### Busca Todos Entregadores
    `GET` `/deliverer`

  Busca todos os entregadores (paginação opcional).
    - `pageNumber` : número de página (opcional)
    - `pageQuantity` : quantidade a ser recebida (opcional)

- ### Criar Entregador
    `POST` `/deliverer`

    Cria um novo entregador.
    ```json
    {
      "name": "nome completo",
      "primaryDocument": "cnpj",
      "birthday": "1999-01-01T00:00:00.000Z",
      "cnh": "cnh",
      "driversLicense": 0
    }
    ```
  os valores para o driversLicense são os seguintes:
  - `0` CNH válida para o seguinte tipo: A
  - `1` CNH válida para o seguinte tipo: B
  - `2` CNH válida para o seguinte tipo: A + B

- ### Enviar Documento para Entregador
  `POST` `/deliverer/document`

  Permite que o entregador envie seu documento.
  
  - Upload de arquivo .png ou .bmp 

## Motorcycle
- ### Criar Moto
  `POST` `/motorcycle`

  Cria uma nova motocicleta.
  ```json
  {
    "model": "modelo da moto",
    "year": "ano da moto",
    "licensePlate": "placa da moto"
  }
  ```

- ### Obter Todas as Motos
  `GET` `/motorcycle`

  Realiza a busca de todas as motos (paginação opcional).
    - `pageNumber` : número de página (opcional)
    - `pageQuantity` : quantidade a ser recebida (opcional)

- ### Obter Moto pela Placa
  `GET` `/motorcycle/license-plate`

  Busca uma moto pela sua placa.
  - `licensePlate`: placa da moto

- ### Alterar a Placa de uma Motocicleta
  `PUT` `/motorcycle/license-plate`

  Altera a placa de uma moto, é verificado se a nova placa já está em uso.
    - `newPlate` : nova placa
    - `motorcycleId` : id da moto

- ### Remover Motocicleta
  `DELETE` `/motorcycle`

  Remove uma moto com o ID especificado.
    - `motorcycleId` : id da moto

## Rental Period
- ### Busca todos os períodos
  `GET` `/rentalPeriod`

  Realiza a busca de todos os períodos (paginação opcional). 
    - `pageNumber` : número de página (opcional)
    - `pageQuantity` : quantidade a ser recebida (opcional)

- ### Cria um novo período de aluguel
  `POST` `/rentalPeriod`

  Cria um novo período de aluguel.
  ```json
  {
    "days": "quantidade de dias",
    "dailyPrice": "valor diário",
    "percentagePenalty": "porcentagem de multa"
  }
  ```
- ### Remover período de aluguel
  `DELETE` `/rentalPeriod`

  Remove um período de aluguel.
    - `rentalPeriodId` : id do período
  
## Rental
- ## Alugar Motocicleta
  `POST` `/rental/rent`

  Aluga uma motocicleta para um entregador por um período de aluguel específico.
    - `delivererId` : id do entregador
    - `motorcycleId` : id da moto
    - `rentalPeriodId` : id do período

- ## Devolver Motocicleta
  `POST` `/rental/return`
  
  Realiza a devolução da moto e faz o cálculo de multa caso necessário.
    - `motorcycleId` : id da moto

## Order
- ### Criar Pedido
  `POST` `/order/create`

  Cria um novo pedido com o preço especificado e notifica todos os entregadores.
    - `price` : valor do pedido

- ### Aceitar Pedido
  `POST` `/order/accept`

  Aceita um pedido atualizando o status do pedido e atribuindo o ID do entregador.
    - `orderId` : id do pedido
    - `delivererId` : id do entregador

- ### Entregar Pedido
  `POST` `/order/delivery`

  Realiza a entrega um pedido atualizando o status do pedido.
  - `orderId` : id do pedido
  - `delivererId` : id do entregador

## Notification
- ### Obter todas as notificações
  `GET` `/notification`

  Realiza a busca de todas as notificações (paginação opcional).
    - `pageNumber` : número de página (opcional)
    - `pageQuantity` : quantidade a ser recebida (opcional)
 
- ### Obter Notificações por Pedido
  `GET` `/notification/order`

  Busca todas as notificações por pedido.
    - `orderId` : id do pedido

- ### Consumir Todas as Notificações
  `GET` `/notification/consume`

  Esta rota consome todas as notificações.

- ## Esquemas de Segurança
  - Tipo: Bearer
  - Descrição: Digite 'Bearer [seu token]' para acessar os endpoints.
  - Nome: Authorization 
  - Em: Cabeçalho
