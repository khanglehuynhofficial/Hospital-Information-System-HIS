pipeline {
    agent any

    tools {
        maven 'Maven-3.9.6'
        jdk 'OpenJDK-17'
    }

    triggers {
        pollSCM('* * * * *')
    }

    environment {
        // ĐÃ ĐIỀN ĐẦY ĐỦ: Đường dẫn chi tiết kho chứa GitHub của Khang
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

        stage('2. Build & Compile') {
            steps {
                echo '=== STEP 2: COMPILING SPRING BOOT APPLICATION ==='
                dir('HisEmrService') {
                    sh 'mvn clean compile'
                }
            }
        }

        stage('3. Run Automated Unit Tests') {
            steps {
                echo '=== STEP 3: EXECUTING UNIT TESTS & CODE COVERAGE ==='
                dir('HisEmrService') {
                    sh 'mvn test'
                }
            }
            post {
                always {
                    dir('HisEmrService') {
                        junit '**/target/surefire-reports/*.xml'
                    }
                }
            }
        }

        stage('4. Static Code Analysis / Quality Gate') {
            steps {
                echo '=== STEP 4: SCANNING CODE QUALITY WITH SONARQUBE ==='
                dir('HisEmrService') {
                    withSonarQubeEnv('SonarQube-Server') {
                        sh "${SCANNER_HOME}/bin/sonar-scanner -Dsonar.projectKey=HIS-Hospital-Information-System -Dsonar.sources=. -Dsonar.java.binaries=**/target/classes"
                    }
                    timeout(time: 5, unit: 'MINUTES') {
                        waitForQualityGate abortPipeline: true
                    }
                }
            }
        }

        stage('5. Package Artifact') {
            steps {
                echo '=== STEP 5: PACKAGING EXECUTABLE APPLICATION ARTIFACT ==='
                dir('HisEmrService') {
                    sh 'mvn package -DskipTests'
                    archiveArtifacts artifacts: '**/target/*.jar', fingerprint: true
                }
            }
        }
    }

    post {
        success {
            echo '=== CI PIPELINE EXECUTED SUCCESSFULLY ==='
            script {
                slackSend(channel: '#his-alerts', color: 'good', message: "🟢 BÁO CÁO: Luồng build ${env.JOB_NAME} [Số #${env.BUILD_NUMBER}] ĐÃ THÀNH CÔNG RỰC RỠ! Bộ quét tĩnh SonarQube đạt trạng thái Quality Gate Passed.")
            }
        }
        failure {
            echo '=== CI PIPELINE FAILED AT SOME STAGES ==='
            script {
                slackSend(channel: '#his-alerts', color: 'danger', message: "🔴 CẢNH BÁO: Luồng build ${env.JOB_NAME} [Số #${env.BUILD_NUMBER}] BỊ THẤT BẠI tại Stage: ${env.STAGE_NAME}. Vui lòng đối soát lại nhật ký Console Output.")
            }
        }
    }
}
