# Hangfire com .NET 10

Este projeto apresenta exemplos de implementação do **Hangfire** em uma Web API desenvolvida com **ASP.NET Core .NET 10**, demonstrando como executar tarefas em segundo plano (*background jobs*).

O Hangfire permite executar tarefas sem bloquear o processamento das requisições HTTP, além de oferecer recursos para agendamento, recorrência, persistência e monitoramento de tarefas.

## 📌 Tipos de Background Jobs

O Hangfire oferece diferentes formas de executar tarefas em segundo plano. Neste projeto, são abordados três tipos principais:

- **Enqueued Jobs:** tarefas enfileiradas para execução assim que um worker estiver disponível.
- **Scheduled Jobs:** tarefas programadas para execução após um intervalo ou em um momento futuro.
- **Recurring Jobs:** tarefas executadas repetidamente conforme uma programação definida.

## 1. Comparação entre os tipos de Jobs

| Característica | Enqueued Job | Scheduled Job | Recurring Job |
|---|---|---|---|
| Execução | Assim que um worker estiver disponível | Após um intervalo ou em um horário futuro | Repetidamente, conforme uma programação |
| Execução automática | Sim | Sim, após o horário programado | Sim, conforme a recorrência configurada |
| Repetição automática | Não, por padrão | Não, por padrão | Sim |
| Exemplo | Processar um pedido | Enviar um lembrete após 10 minutos | Gerar um relatório diariamente |
| API do Hangfire | `BackgroundJob.Enqueue()` | `BackgroundJob.Schedule()` | `RecurringJob.AddOrUpdate()` |
| Principal utilização | Processamento imediato em segundo plano | Execução adiada | Automação periódica |

### 1.1. Enqueued Job

Um *Enqueued Job* é uma tarefa adicionada à fila de processamento do Hangfire, ficando disponível para execução assim que um worker puder processá-la.

**Exemplos de utilização:**

- Processar pedidos após a confirmação de uma compra.
- Enviar e-mails de confirmação.
- Processar arquivos enviados por usuários.
- Executar operações que não precisam ser concluídas antes de responder à requisição HTTP.

**Exemplo em C#:**

```csharp
BackgroundJob.Enqueue<IBackgroundJobsService>(
    service => service.ExecuteExample());
```

Nesse exemplo, o Hangfire registra a tarefa para processamento em segundo plano. A requisição HTTP não precisa aguardar a conclusão da tarefa.

### 1.2. Scheduled Job

Um *Scheduled Job* é uma tarefa programada para execução futura, após um intervalo definido ou em um horário determinado.

**Exemplos de utilização:**

- Enviar um lembrete após 10 minutos.
- Cancelar um pedido não pago após 30 minutos.
- Enviar uma notificação em um momento específico.
- Executar uma operação posteriormente.

**Exemplo em C#:**

```csharp
BackgroundJob.Schedule<IBackgroundJobsService>(
    service => service.ExecuteExample(),
    TimeSpan.FromMinutes(10));
```

Nesse exemplo, a tarefa fica programada para execução após 10 minutos. Quando o horário é alcançado, o Hangfire a disponibiliza para processamento.

### 1.3. Recurring Job

Um *Recurring Job* é uma tarefa configurada para ser executada repetidamente, seguindo uma expressão cron ou uma programação predefinida.

**Exemplos de utilização:**

- Sincronizar informações com o SAP a cada 15 minutos.
- Gerar relatórios diariamente.
- Limpar registros expirados durante a madrugada.
- Verificar pedidos pendentes periodicamente.

**Exemplo em C#:**

```csharp
RecurringJob.AddOrUpdate<IBackgroundJobsService>(
    "daily-report",
    service => service.ExecuteExample(),
    "0 8 * * *");
```

Nesse exemplo, a tarefa é programada para execução diária às 08h00, de acordo com o fuso horário configurado para o agendador.

A expressão cron utilizada é:

```text
0 8 * * *
```

Sua interpretação é: todos os dias, às 08h00.

O identificador `daily-report` permite que o Hangfire reconheça o job recorrente e atualize sua configuração quando necessário.

---

## 2. Qual tipo de Job utilizar?

A escolha depende do momento em que a tarefa precisa ser executada e da necessidade de repetição.

| Necessidade do sistema | Tipo recomendado |
|---|---|
| Processar uma requisição em segundo plano assim que possível | Enqueued |
| Processar uma tarefa após 10 minutos | Scheduled |
| Sincronizar dados com o SAP a cada 15 minutos | Recurring |
| Gerar relatórios todas as noites | Recurring |
| Enviar um e-mail de boas-vindas após o cadastro | Enqueued |
| Notificar um cliente sobre um pedido não pago após 30 minutos | Scheduled |
| Consultar notas fiscais pendentes a cada hora | Recurring |

### Recomendações práticas

**Utilize Enqueued quando:**
- A tarefa precisa começar assim que houver capacidade de processamento.
- A requisição HTTP não deve aguardar a conclusão de uma operação demorada.
- O processamento pode ocorrer independentemente da resposta enviada ao cliente.

**Utilize Scheduled quando:**
- A tarefa precisa aguardar um intervalo específico.
- A execução deve ocorrer em um momento futuro.
- Existe uma regra de negócio que determina quando o processamento deve começar.

**Utilize Recurring quando:**
- A tarefa precisa ser executada repetidamente.
- Existe uma frequência definida para a operação.
- O sistema precisa automatizar verificações, sincronizações ou rotinas administrativas.

## 3. Fluxo de execução

Os três tipos de jobs possuem comportamentos diferentes, mas compartilham o mecanismo de processamento do Hangfire.

```text
ENQUEUED
   |
   v
Aguardando um worker disponível
   |
   v
Execução da tarefa
   |
   v
Concluída ou com falha


SCHEDULED
   |
   v
Aguardando o horário programado
   |
   v
ENQUEUED
   |
   v
Execução da tarefa


RECURRING
   |
   v
Aguardando o próximo horário
   |
   v
ENQUEUED
   |
   v
Execução da ocorrência
   |
   v
Aguardando a próxima ocorrência
```

**Observação:** uma tarefa enfileirada não é necessariamente executada de forma imediata. Sua execução depende da disponibilidade dos workers, da configuração das filas e das condições do ambiente.

## 4. Filas de processamento (Queues)

O Hangfire permite organizar as tarefas em filas diferentes, separando os tipos de processamento de acordo com as necessidades do sistema.

Exemplos de nomes de filas:

- `default`: fila padrão.
- `main-queue`: processamento principal.
- `priority-queue`: tarefas prioritárias.
- `sap-queue`: integração com o SAP.

A configuração de filas é independente do tipo de job. Uma tarefa enfileirada, agendada ou recorrente pode utilizar uma fila específica, desde que sua configuração seja compatível com o mecanismo de enfileiramento utilizado.

O servidor Hangfire deve estar configurado para monitorar as filas nas quais as tarefas serão disponibilizadas.

## 5. Tecnologias utilizadas

- C#
- ASP.NET Core Web API
- .NET 10
- Hangfire
- SQL Server
- Swagger / OpenAPI

## 6. Referências

- [Documentação oficial do Hangfire](https://docs.hangfire.io/en/latest/)
- [Background Jobs](https://docs.hangfire.io/en/latest/background-methods/index.html)
- [Recurring Jobs](https://docs.hangfire.io/en/latest/background-methods/performing-recurrent-tasks.html)
- [Configuração de filas](https://docs.hangfire.io/en/latest/background-processing/configuring-queues.html)

---

**Objetivo do projeto:** demonstrar os principais mecanismos de processamento em segundo plano utilizando Hangfire em uma Web API .NET, com exemplos que podem ser adaptados a aplicações reais.
