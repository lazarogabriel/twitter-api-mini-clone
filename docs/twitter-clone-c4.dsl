workspace {

  model {
    user = person "Usuario" {
      description "Persona que interactúa con el clon de Twitter"
    }

    system = softwareSystem "Clon de Twitter (Backend)" {

      container_api = container "Presentation Layer" {
        technology "ASP.NET Core REST API (Dockerized)"
        description "Capa HTTP que expone endpoints REST, ejecutándose como contenedor Docker"
      }

      container_application = container "Application Layer" {
        technology ".NET Core"
        description "Contiene servicios, lógica de aplicación y contratos"

        tweet_service = component "TweetService" {
          technology "C#"
          description "Gestiona publicación y recuperación de tweets"
        }

        user_service = component "UserService" {
          technology "C#"
          description "Gestiona el registro y consulta de usuarios"
        }

        follow_service = component "FollowService" {
          technology "C#"
          description "Gestiona relaciones de seguimiento entre usuarios"
        }

        timeline_query_service = component "TimelineQueryService" {
          technology "C#"
          description "Obtiene el timeline del usuario"
        }

        container_api -> this "Llama a servicios de aplicación"
      }

      container_domain = container "Domain Layer" {
        technology ".NET Core (Entidades de dominio)"
        description "Define entidades y lógica de negocio"
        container_application -> this "Usa entidades y reglas del dominio"
      }

      container_database = container "PostgreSQL Database" {
        technology "PostgreSQL 15 (Dockerized)"
        description "Base de datos relacional desplegada como contenedor Docker"
      }

      container_infrastructure = container "Infrastructure Layer" {
        technology "EF Core + PostgreSQL Driver"
        description "Implementación de acceso a datos usando Entity Framework, ejecutada en contenedor API"
        container_application -> this "Accede a persistencia"
        this -> container_database "Lee y escribe datos"
      }

      container_api -> tweet_service "Llama para crear/leer tweets"
      container_api -> user_service "Llama para registrar/consultar usuarios"
      container_api -> follow_service "Llama para seguir/dejar de seguir"
      container_api -> timeline_query_service "Llama para obtener timeline"

      tweet_service -> container_infrastructure "Usa ITwitterApiDbContext"
      user_service -> container_infrastructure "Usa ITwitterApiDbContext"
      follow_service -> container_infrastructure "Usa ITwitterApiDbContext"
      timeline_query_service -> container_infrastructure "Usa ITwitterApiDbContext"
    }

    http_client = softwareSystem "Cliente HTTP (Frontend o Swagger)" {
      description "Interfaz que el usuario usa para enviar peticiones HTTP a la API, como Swagger, Postman o una app web"
    }

    // Relaciones entre actores y sistemas (¡ya definidos!)
    user -> http_client "Usa para interactuar con la API"
    http_client -> container_api "Hace peticiones HTTP con x-user-id"
  }

  views {
      systemContext system {
        include system
        include user
        include http_client
        autolayout lr
        animation {
          user
          http_client
          system
        }
  }

    container system {
      include *
      autolayout lr
      animation {
        container_api
      }
      animation {
        container_application
        container_domain
        container_infrastructure
      }
      animation {
        container_database
      }
    }

    component container_application {
      include *
      autolayout lr

      animation {
        tweet_service
        user_service
      }
      animation {
        follow_service
        timeline_query_service
      }
    }

    theme default
  }
}