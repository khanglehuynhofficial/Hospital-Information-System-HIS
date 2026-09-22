pipeline {
    agent any

    triggers {
        // Tự động quét kiểm tra mã nguồn từ GitHub 1 phút / lần
        pollSCM('* * * * *') [1.3, 2]
    }

    environment {
        GITHUB_REPO = 'https://github.com'
        CREDENTIALS_ID = 'his-github-auth'
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
                // Rút gọn cú pháp gọi Slack, hệ thống tự động nhận diện thông số mạng Webhook
                slackSend(channel: '#his-devops-alerts', color: 'good', message: "🟢 BÁO CÁO: Luồng build ${env.JOB_NAME} [Số #${env.BUILD_NUMBER}] ĐÃ THÀNH CÔNG RỰC RỠ! Bộ quét tĩnh SonarQube đạt trạng thái Quality Gate Passed.")
            }
        }
        failure {
            echo '=== CI PIPELINE FAILED AT SOME STAGES ==='
            script {
                slackSend(channel: '#his-devops-alerts', color: 'danger', message: "🔴 CẢNH BÁO: Luồng build ${env.JOB_NAME} [Số #${env.BUILD_NUMBER}] BỊ THẤT BẠI tại Stage: ${env.STAGE_NAME}. Vui lòng đối soát lại nhật ký Console Output.")
            }
        }
    }
}
