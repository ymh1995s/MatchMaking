pipeline {
    agent any

    stages {
		stage('Clean') {
		  steps {
			deleteDir()   // 현재 워크스페이스 싹 삭제
		  }
		}
	
		stage('Checkout') {
		  steps {
			git url: 'https://github.com/ymh1995s/MatchMaking.git', branch: '*/main'
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