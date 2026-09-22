pipeline {
    agent any

    triggers {
        pollSCM('* * * * *')
    }

    environment {
        GITHUB_REPO = 'https://github.com/khanglehuynhofficial/Hospital-Information-System-HIS'
        CREDENTIALS_ID = 'his-github-auth'
        SCANNER_HOME = tool 'SonarQubeScanner'
    }

    stages {
        stage('1. Checkout Source Code') {
            steps {
                echo '=== STEP 1: PULLING LATEST CODE FROM GITHUB ==='
                checkout([$class: 'GitSCM',
                    branches: [[name: '*/develop']],
                    extensions: [],
                    userRemoteConfigs: [[credentialsId: "${CREDENTIALS_ID}", url: "${GITHUB_REPO}"]]
                ])
            }
        }

        stage('2. Build & Compile (.NET)') {
            steps {
                echo '=== STEP 2: COMPILING .NET APPLICATION ==='
                // Sử dụng lệnh dotnet chuẩn của hệ thống thay cho Maven
                sh 'dotnet build'
            }
        }

        stage('3. Run Static Code Analysis') {
            steps {
                echo '=== STEP 3: SCANNING CODE QUALITY WITH SONARQUBE ==='
                withSonarQubeEnv('SonarQube-Server') {
                    sh "${SCANNER_HOME}/bin/sonar-scanner -Dsonar.projectKey=HIS-Hospital-Information-System -Dsonar.sources=."
                }
            }
        }
    }
        post {
        success {
            echo '=== CI PIPELINE EXECUTED SUCCESSFULLY ==='
            script {
                slackSend(
                    tokenCredentialId: 'slack-token-secret',
                    channel: '#his-devops-alerts', // ĐÃ SỬA: Đổi sang kênh mới của bạn
                    color: 'good',
                    message: "🟢 BÁO CÁO: Luồng build ${env.JOB_NAME} [Số #${env.BUILD_NUMBER}] ĐÃ THÀNH CÔNG RỰC RỠ! Bộ quét tĩnh SonarQube đạt trạng thái Quality Gate Passed."
                )
            }
        }
        failure {
            echo '=== CI PIPELINE FAILED AT SOME STAGES ==='
            script {
                slackSend(
                    tokenCredentialId: 'slack-token-secret',
                    channel: '#his-devops-alerts', // ĐÃ SỬA: Đổi sang kênh mới của bạn
                    color: 'danger',
                    message: "🔴 CẢNH BÁO: Luồng build ${env.JOB_NAME} [Số #${env.BUILD_NUMBER}] BỊ THẤT BẠI tại Stage: ${env.STAGE_NAME}. Vui lòng đối soát lại nhật ký Console Output."
                )
            }
        }
    }
}
