pipeline {
    agent any

    triggers {
        // Tự động quét kiểm tra mã nguồn từ GitHub 1 phút / lần
        pollSCM('* * * * *')
    }

    environment {
        GITHUB_REPO = 'https://github.com/khanglehuynhofficial/Hospital-Information-System-HIS.git'
        CREDENTIALS_ID = 'his-github-auth'
        SLACK_CREDENTIALS_ID = 'slack-token-secret'
        SLACK_CHANNEL = '#his-devops-alerts'
        // ÉP CỨNG ĐƯỜNG DẪN: Bảo đảm gọi trúng thư mục lõi đã cài đặt trên Ubuntu
        SCANNER_HOME = '/opt/sonar-scanner'
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

        stage('2. Static Code Analysis / Quality Gate') {
            steps {
                echo '=== STEP 2: SCANNING CODE QUALITY WITH SONARQUBE ==='
                withSonarQubeEnv('SonarQube-Server') {
                    // Gọi trực tiếp đường dẫn bin của bộ quét để xử lý triệt để lỗi exit code 127
                    sh "${SCANNER_HOME}/bin/sonar-scanner -Dsonar.projectKey=HIS-Hospital-Information-System -Dsonar.sources=. -Dsonar.sourceEncoding=UTF-8"
                }
            }
        }
    }

    post {
        success {
            echo '=== CI PIPELINE EXECUTED SUCCESSFULLY ==='
            script {
                slackSend(
                    tokenCredentialId: env.SLACK_CREDENTIALS_ID,
                    channel: env.SLACK_CHANNEL,
                    color: 'good',
                    message: "🟢 BÁO CÁO: Luồng build ${env.JOB_NAME} [Số #${env.BUILD_NUMBER}] đã thành công. SonarQube đã hoàn tất quét mã nguồn. ${env.BUILD_URL}"
                )
            }
        }
        failure {
            echo '=== CI PIPELINE FAILED AT SOME STAGES ==='
            script {
                slackSend(
                    tokenCredentialId: env.SLACK_CREDENTIALS_ID,
                    channel: env.SLACK_CHANNEL,
                    color: 'danger',
                    message: "🔴 CẢNH BÁO: Luồng build ${env.JOB_NAME} [Số #${env.BUILD_NUMBER}] thất bại. Vui lòng đối soát Console Output. ${env.BUILD_URL}"
                )
            }
        }
    }
}
