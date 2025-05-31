workspace {

    model {
        user = person "Usuario" {
            description "Persona que interactúa con la app"
        }

        system = softwareSystem "Twitter Mini Clone" {
            user -> this "Usa"
            
            webClient = container "Web Client" {
                description "Cliente web"
                technology "React.js"
                tag "WebBrowser"
            }
    
            mobileApp = container "Mobile App" {
                description "Aplicación móvil"
                technology "Flutter o React Native"
                tag MobileApp
            }
            apiGateway = container "API Gateway" {
                description "Entrada centralizada con autenticación, rate limiting, balanceo y cache"
                technology "NGINX + Redis"
                tag Gateway
            }

            tweetService = container "TweetService API" {
                description "Crea tweets"
                technology ".NET Web API"
                tag Microservice
            }

            followService = container "FollowService API" {
                description "Gestión de follows"
                technology ".NET Web API"
                tag Microservice
            }

            timelineService = container "TimelineService API" {
                description "Devuelve tweets de seguidos"
                technology ".NET 9 Web API"
                tag Microservice
            }

            writeDb = container "PostgreSQL Write DB" {
                description "Base de datos principal"
                technology "PostgreSQL"
                tag "Database"
            }

            readReplica = container "PostgreSQL Read Replica" {
                description "Replica de solo lectura para timeline"
                technology "PostgreSQL"
                tag "Database"
            }

            redisCache = container "Redis Cache" {
                description "Cache de timelines recientes"
                technology "Redis"
                tag "Database"
            }

            syncService = container "CDC / Sync Service" {
                description "Sincroniza DB principal con réplica"
                technology "Debezium o similar"
                tag Gateway
            }

            webClient -> apiGateway "Consume API"
            mobileApp -> apiGateway "Consume API"

            apiGateway -> tweetService "Redirige POSTs de tweets"
            apiGateway -> followService "Redirige follow/unfollow"
            apiGateway -> timelineService "Redirige lecturas del timeline"

            tweetService -> writeDb "Escribe tweets"
            followService -> writeDb "Escribe follows"
            timelineService -> readReplica "Lee para timeline"
            timelineService -> redisCache "Consulta y actualiza cache"

            writeDb -> syncService "Emite cambios"
            syncService -> readReplica "Replica datos"
        }
    
        user -> webClient "Usa"
        user -> mobileApp "Usa"
    }

    views {
        systemContext system {
            include *
            autolayout lr
            title "Contexto: Twitter Mini Clone"
        }

        container system {
            include *
            autolayout lr
            title "Contenedores: Twitter Mini Clone"
        }
    
        theme default
    
        styles {
            element "Container" {
                shape box
            }

            element "Person" {
                shape person
            }

            element "PostgreSQL Write DB" {
                shape cylinder
            }

            element "Database" {
                shape cylinder
            }

            element "WebBrowser" {
                shape webBrowser
            }

            element "MobileApp" {
                shape MobileDeviceLandscape
            }

            element "Microservice" {
                background #1E90FF
                shape Hexagon
            }
      
            element "Gateway" {
                shape pipe
            }
        }
    }
}
