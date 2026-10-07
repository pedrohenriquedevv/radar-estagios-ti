# Radar de Estágios TI

Monitor de vagas de estágio relacionadas à tecnologia em Catalão, Goiás, e Uberlândia, Minas Gerais.

## Objetivo

Construir um pipeline responsável e testável para coletar, normalizar, validar, classificar, pontuar e armazenar oportunidades compatíveis com um perfil de estudante de Sistemas de Informação.

## Escopo inicial

O MVP deverá monitorar vagas de estágio em tecnologia localizadas em:

- Catalão, GO;
- Uberlândia, MG.

Vagas remotas e oportunidades em outras cidades não fazem parte do escopo inicial.

## Estado atual

O projeto está em fase de inicialização.

Neste momento, a solução contém:

- uma aplicação de linha de comando em C#;
- um projeto de testes automatizados com xUnit;
- configuração para .NET 10;
- estrutura inicial de código-fonte e testes.

As regras de negócio, a coleta de vagas, a persistência e as notificações ainda não foram implementadas.

## Tecnologias iniciais

- C#;
- .NET 10;
- xUnit;
- Git.

## Estrutura

```text
radar-estagios-ti/
├── src/
│   └── RadarEstagios.Cli/
├── tests/
│   └── RadarEstagios.Tests/
├── .gitignore
├── README.md
└── RadarEstagios.slnx

```

## Executar o projeto

```powershell
dotnet run --project .\src\RadarEstagios.Cli\RadarEstagios.Cli.csproj
```

## Executar os testes

```powershell
dotnet test .\RadarEstagios.slnx
```

## Compilar a solução

```powershell
dotnet build .\RadarEstagios.slnx
```

## Princípios do projeto

- trabalhar com uma pequena tarefa por vez;
- preservar a explicabilidade das classificações e pontuações;
- utilizar coleta responsável;
- não armazenar credenciais ou dados desnecessários;
- manter testes reproduzíveis;
- evitar complexidade antecipada;
- não considerar uma implementação concluída sem evidências de validação.