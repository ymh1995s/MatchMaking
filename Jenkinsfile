pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                checkout scm
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