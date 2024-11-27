import org.gradle.api.tasks.testing.logging.TestExceptionFormat

plugins {
    alias(libs.plugins.android.application)
    id("de.mannodermaus.android-junit5") version "1.11.2.0"
}

android {
    namespace = "com.example.dairemote_app"
    compileSdk = 34

    defaultConfig {
        applicationId = "com.example.dairemote_app"
        minSdk = 24
        targetSdk = 34
        versionCode = 1
        versionName = "1.0"

        testInstrumentationRunner = "androidx.test.runner.AndroidJUnitRunner"
    }

    buildTypes {
        release {
            isMinifyEnabled = false
            proguardFiles(
                getDefaultProguardFile("proguard-android-optimize.txt"),
                "proguard-rules.pro"
            )
        }
    }
    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_1_8
        targetCompatibility = JavaVersion.VERSION_1_8
    }
}

dependencies {

    implementation(libs.appcompat)
    implementation(libs.material)
    implementation(libs.activity)
    implementation(libs.constraintlayout)
    implementation(libs.recyclerview)
    testImplementation(libs.junit)
    testImplementation(libs.junit.jupiter)
    androidTestImplementation(libs.ext.junit)
    androidTestImplementation(libs.espresso.core)
    testImplementation("org.mockito:mockito-core:5.14.2")
}

// Test Logging
tasks.withType<Test> {
    testLogging {
        exceptionFormat = TestExceptionFormat.FULL
        events("started", "skipped", "passed", "failed")
        showStandardStreams = true
    }

    var totalTests = 0
    var passedTests = 0
    var failedTests = 0
    var skippedTests = 0

    // Track test statuses
    addTestListener(object : TestListener {
        override fun beforeTest(testDescriptor: TestDescriptor) {
            // Do nothing
        }

        override fun afterTest(testDescriptor: TestDescriptor, result: TestResult) {
            totalTests++
            when (result.resultType) {
                TestResult.ResultType.SUCCESS -> passedTests++
                TestResult.ResultType.FAILURE -> failedTests++
                TestResult.ResultType.SKIPPED -> skippedTests++
            }
        }

        override fun beforeSuite(suite: TestDescriptor) {
            // Do nothing
        }

        override fun afterSuite(suite: TestDescriptor, result: TestResult) {
            if (suite.parent == null) {
                println(
                    """
                    -------------
                    Test Summary:
                    -------------
                    Total: $totalTests
                    Passed: $passedTests
                    Failed: $failedTests
                    Skipped: $skippedTests
                    -------------
                    """.trimIndent()
                )
            }
        }
    })
}