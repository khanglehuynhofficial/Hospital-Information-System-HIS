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
                echo '=== STEP 5: PACKAGING EXECUTABLE APPLICATION ARTIFACT ==='
                sh 'mvn package -DskipTests'
                archiveArtifacts artifacts: '**/target/*.jar', fingerprint: true
            }
        }
    }
    
    post {
        success {
            echo '=== CI PIPELINE EXECUTED SUCCESSFULLY ==='
        }
        failure {
            echo '=== CI PIPELINE FAILED AT SOME STAGES ==='
        }
    }
}
