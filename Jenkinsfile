pipeline {
    agent any

    options {
        skipDefaultCheckout(true)   // <-- 핵심: 자동 "Declarative: Checkout SCM" 끔
    }

    stages {
        stage('Clean') {
            steps {
                deleteDir()
            }
        }

        stage('Checkout') {
            steps {
                git branch: 'main', url: 'https://github.com/ymh1995s/MatchMaking.git'
            }
        }

        stage('Build Docker Image') {
            steps {
                sh 'docker build -t matchmaking-api .'
            }
        }

        stage('Stop Old Container') {
            steps {
                sh 'docker stop matchmaking-api || true'
                sh 'docker rm matchmaking-api || true'
            }
        }

        stage('Run New Container') {
            steps {
                sh '''
                docker run -d \
                -p 8080:8080 \
                --name matchmaking-api \
                matchmaking-api
                '''
            }
        }
    }
}