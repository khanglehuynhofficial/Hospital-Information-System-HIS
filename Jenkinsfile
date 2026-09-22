pipeline {
    agent any
    
    tools {
        maven 'Maven-3.9.6'
        jdk 'OpenJDK-17'
    }
    
    environment {
        GITHUB_REPO = 'https://github.com/khanglehuynhofficial/Hospital-Information-System-HIS.git'
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
        
        stage('2. Build & Compile') {
            steps {
                echo '=== STEP 2: COMPILING SPRING BOOT APPLICATION ==='
                sh 'mvn clean compile'
            }
        }
        
        stage('3. Run Automated Unit Tests') {
            steps {
                echo '=== STEP 3: EXECUTING UNIT TESTS & CODE COVERAGE ==='
                sh 'mvn test'
            }
            post {
                always {
                    junit '**/target/surefire-reports/*.xml'
                }
            }
        }
        
        stage('4. Static Code Analysis / Quality Gate') {
            steps {
                echo '=== STEP 4: SCANNING CODE QUALITY WITH SONARQUBE ==='
                withSonarQubeEnv('SonarQube-Server') {
                    sh "${SCANNER_HOME}/bin/sonar-scanner -Dsonar.projectKey=HIS-Hospital-Information-System -Dsonar.sources=. -Dsonar.java.binaries=**/target/classes"
                }
                timeout(time: 5, unit: 'MINUTES') {
                    waitForQualityGate abortPipeline: true
                }
            }
        }
        
        stage('5. Package Artifact') {
            steps {
       pipeline {
    agent any

    tools {
        maven 'Maven-3.9.6'
        jdk 'OpenJDK-17'
    }

    triggers {
        // Cấu hình cơ chế chủ động quét GitHub 1 phút/lần thay thế Webhook
        pollSCM('* * * * *')
    }

    environment {
        GITHUB_REPO = 'https://github.com'
        CREDENTIALS_ID = 'his-github-auth'
        SCANNER_HOME = tool 'SonarQubeScanner'
    }

    stages {
        stage('1. Checkout Source Code') {
            steps {
                echo '=== STEP 1: PULLING LATEST CODE FROM GITHUB ==='
                checkout([\$class: 'GitSCM',
                    branches: [[name: '*/develop']],
                    extensions: [],
                    userRemoteConfigs: [[credentialsId: "\({CREDENTIALS_ID}", url: "\){GITHUB_REPO}"]]
                ])
            }
        }

        stage('2. Build & Compile') {
            steps {
                echo '=== STEP 2: COMPILING SPRING BOOT APPLICATION ==='
                sh 'mvn clean compile'
            }
        }

        stage('3. Run Automated Unit Tests') {
            steps {
                echo '=== STEP 3: EXECUTING UNIT TESTS & CODE COVERAGE ==='
                sh 'mvn test'
            }
            post {
                always {
                    junit '**/target/surefire-reports/*.xml'
                }
            }
        }

        stage('4. Static Code Analysis / Quality Gate') {
            steps {
                echo '=== STEP 4: SCANNING CODE QUALITY WITH SONARQUBE ==='
                withSonarQubeEnv('SonarQube-Server') {
                    sh "\${SCANNER_HOME}/bin/sonar-scanner -Dsonar.projectKey=HIS-Hospital-Information-System -Dsonar.sources=. -Dsonar.java.binaries=**/target/classes"
                }
                timeout(time: 5, unit: 'MINUTES') {
                    waitForQualityGate abortPipeline: true
                }
            }
        }

        stage('5. Package Artifact') {
            steps {
                echo '=== STEP 5: PACKAGING EXECUTABLE APPLICATION ARTIFACT ==='
                sh 'mvn package -DskipTests'
                archiveArtifacts artifacts: '**/target/*.jar', fingerprint: true
            }
        }
    }

    // KHUNG CẤU HÌNH SLACK CHÈN THÊM VÀO ĐUÔI FILE - GIỮ NGUYÊN TOÀN BỘ PHẦN TRÊN
    post {
        success {
            echo '=== CI PIPELINE EXECUTED SUCCESSFULLY ==='
            script {
                // Tự động gửi thông báo viền xanh lá khi toàn bộ 5 stage thành công
                slackSend(
            channel: '#his-alerts', 
            color: 'good',
            message: "🟢 BÁO CÁO: Luồng build ${env.JOB_NAME} [Số #${env.BUILD_NUMBER}] ĐÃ THÀNH CÔNG RỰC RỠ! Bộ quét tĩnh SonarQube đạt trạng thái Quality Gate Passed."
		)
          \pipeline {
    agent any

    tools {
        maven 'Maven-3.9.6'
        jdk 'OpenJDK-17'
    }

    triggers {
        // Cấu hình cơ chế chủ động quét GitHub 1 phút/lần thay thế Webhook
        pollSCM('* * * * *') [1.3, 2]
    }

    environment {
        GITHUB_REPO = 'https://github.com/khanglehuynhofficial/Hospital-Information-System-HIS.git'
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

        stage('2. Build & Compile') {
            steps {
                echo '=== STEP 2: COMPILING SPRING BOOT APPLICATION ==='
                sh 'mvn clean compile'
            }
        }

        stage('3. Run Automated Unit Tests') {
            steps {
                echo '=== STEP 3: EXECUTING UNIT TESTS & CODE COVERAGE ==='
                sh 'mvn test'
            }
            post {
                always {
                    junit '**/target/surefire-reports/*.xml'
                }
            }
        }

        stage('4. Static Code Analysis / Quality Gate') {
            steps {
                echo '=== STEP 4: SCANNING CODE QUALITY WITH SONARQUBE ==='
                withSonarQubeEnv('SonarQube-Server') {
                    sh "${SCANNER_HOME}/bin/sonar-scanner -Dsonar.projectKey=HIS-Hospital-Information-System -Dsonar.sources=. -Dsonar.java.binaries=**/target/classes"
                }
                timeout(time: 5, unit: 'MINUTES') {
                    waitForQualityGate abortPipeline: true
                }
            }
        }

        stage('5. Package Artifact') {
            steps {
                echo '=== STEP 5: PACKAGING EXECUTABLE APPLICATION ARTIFACT ==='
                sh 'mvn package -DskipTests'
                archiveArtifacts artifacts: '**/target/*.jar', fingerprint: true
            }
        }
    }

    post {
        success {
            echo '=== CI PIPELINE EXECUTED SUCCESSFULLY ==='
            script {
                // Tự động gửi thông báo viền xanh lá khi toàn bộ 5 stage thành công
                slackSend(
                    tokenCredentialId: 'slack-token-secret',
                    channel: '#his-alerts',
                    color: 'good',
                    message: "🟢 BÁO CÁO: Luồng build ${env.JOB_NAME} [Số #${env.BUILD_NUMBER}] ĐÃ THÀNH CÔNG RỰC RỠ! Bộ quét tĩnh SonarQube đạt trạng thái Quality Gate Passed."
                )
            }
        }
        failure {
            echo '=== CI PIPELINE FAILED AT SOME STAGES ==='
            script {
                // Tự động gửi thông báo viền đỏ cảnh báo khi có stage bị gãy luồng
                slackSend(
                    tokenCredentialId: 'slack-token-secret',
                    channel: '#his-alerts',
                    color: 'danger',
                    message: "🔴 CẢNH BÁO: Luồng build ${env.JOB_NAME} [Số #${env.BUILD_NUMBER}] BỊ THẤT BẠI tại Stage: ${env.STAGE_NAME}. Vui lòng đối soát lại nhật ký Console Output."
                )
            }
        }
    }
}

